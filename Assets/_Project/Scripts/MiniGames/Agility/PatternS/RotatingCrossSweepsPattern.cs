using System.Collections;
using UnityEngine;

public class RotatingCrossSweepsPattern : PatternBehaviour
{
    [Header("Prefabs")]
    public GameObject tokenPrefab; // PF_SweeperToken (body hazard)

    [Header("Entrances (two different gates)")]
    public Entrance gateA = Entrance.NorthEast; // "сверху справа"
    public Entrance gateB = Entrance.SouthEast; // "снизу справа"

    [Header("Timing")]
    public float stepTelegraph = 0.25f;     // пауза перед шагом
    public float passSeconds = 1.4f;        // длительность проезда через арену
    public float betweenPassPause = 0.15f;  // пауза между A и B
    public float rotateSeconds = 0.35f;     // "плавно перемещались" на следующий угол
    public int steps = 4;                  // 3-4 шага = ~10-15 секунд
    public float stepAngleDeg = 45f;       // поворот траекторий на 45°

    [Header("Geometry")]
    public float passDistance = 6.0f;      // радиус до точки "края" траектории (должно перекрывать арену)

    [Header("Return")]
    public float returnSpacing = 0.1f;

    public override IEnumerator Run()
    {
        if (tokenPrefab == null) yield break;

        Transform gatePointA = Ctx.entrances.Get(gateA);
        Transform gatePointB = Ctx.entrances.Get(gateB);
        if (gatePointA == null || gatePointB == null) yield break;

        // Берём XZ от центра, но Y фиксируем по воротам (чтобы не было скачков по высоте)
        Vector3 center = (Ctx.arenaCenter != null) ? Ctx.arenaCenter.position : Vector3.zero;
        center.y = gatePointA.position.y;

        // Gate controllers (если есть)
        GateController gateCtrlA = gatePointA.GetComponentInChildren<GateController>();
        GateController gateCtrlB = gatePointB.GetComponentInChildren<GateController>();
        if (gateCtrlA != null) yield return gateCtrlA.Open();
        if (gateCtrlB != null) yield return gateCtrlB.Open();

        // Spawn tokens у двух разных ворот
        TokenAgent tokenA = SpawnToken(gatePointA.position + new Vector3(-returnSpacing, 0f, 0f));
        TokenAgent tokenB = SpawnToken(gatePointB.position + new Vector3(+returnSpacing, 0f, 0f));

        // Стартовый угол: диагонали (X-образный "крест-накрест")
        // A: NE↔SW (45°), B: SE↔NW (-45°)
        float angleA = 45f * Mathf.Deg2Rad;

        // Стартовые позиции: подтянуть токены на соответствующие "края" своих линий
        // На шаге 0 обе стартуют со стороны "плюс" (правая сторона), дальше будут чередовать
        yield return MoveTo(tokenA, StartPoint(center, angleA, +1f));
        yield return MoveTo(tokenB, StartPoint(center, angleA - Mathf.PI * 0.5f, +1f));

        for (int s = 0; s < steps; s++)
        {
            // знак стартовой стороны:  +1, -1, +1, -1 ... (чтобы каждый шаг пересекали центр и "перебегали" на другой край)
            float startSign = (s % 2 == 0) ? +1f : -1f;

            float aA = angleA;
            float aB = angleA - Mathf.PI * 0.5f; // перпендикуляр

            Vector3 A0 = StartPoint(center, aA, startSign);
            Vector3 A1 = EndPoint(center, aA, startSign);

            Vector3 B0 = StartPoint(center, aB, startSign);
            Vector3 B1 = EndPoint(center, aB, startSign);

            if (stepTelegraph > 0f)
                yield return new WaitForSeconds(stepTelegraph);

            // По очереди пересекают центр
            yield return MoveLinear(tokenA, A0, A1, passSeconds);
            if (betweenPassPause > 0f) yield return new WaitForSeconds(betweenPassPause);

            yield return MoveLinear(tokenB, B0, B1, passSeconds);
            if (betweenPassPause > 0f) yield return new WaitForSeconds(betweenPassPause);

            // Плавный "поворот" на следующий угол: переезжаем на новые стартовые точки
            float nextAngleA = angleA + stepAngleDeg * Mathf.Deg2Rad;

            // на следующем шаге знак стартовой стороны меняется
            float nextSign = ((s + 1) % 2 == 0) ? +1f : -1f;

            Vector3 nextAStart = StartPoint(center, nextAngleA, nextSign);
            Vector3 nextBStart = StartPoint(center, nextAngleA - Mathf.PI * 0.5f, nextSign);

            if (rotateSeconds > 0f)
            {
                yield return MoveLinear(tokenA, tokenA.transform.position, nextAStart, rotateSeconds);
                yield return MoveLinear(tokenB, tokenB.transform.position, nextBStart, rotateSeconds);
            }
            else
            {
                tokenA.transform.position = nextAStart;
                tokenB.transform.position = nextBStart;
            }

            angleA = nextAngleA;
        }

        // Вернуться к своим воротам
        Vector3 retA = gatePointA.position + new Vector3(-returnSpacing, 0f, 0f);
        Vector3 retB = gatePointB.position + new Vector3(+returnSpacing, 0f, 0f);
        yield return MoveTo(tokenA, retA);
        yield return MoveTo(tokenB, retB);

        // Уехать за ворота
        if (tokenA != null) Destroy(tokenA.gameObject);
        if (tokenB != null) Destroy(tokenB.gameObject);

        // Закрыть ворота
        if (gateCtrlA != null) yield return gateCtrlA.Close();
        if (gateCtrlB != null) yield return gateCtrlB.Close();
    }

    private TokenAgent SpawnToken(Vector3 pos)
    {
        var go = Instantiate(tokenPrefab, pos, Quaternion.identity);
        var agent = go.GetComponent<TokenAgent>();
        if (agent == null) agent = go.AddComponent<TokenAgent>();
        return agent;
    }

    private IEnumerator MoveTo(TokenAgent agent, Vector3 target)
    {
        if (agent == null) yield break;
        // TokenAgent.MoveTo уже плавный и читабельный
        yield return agent.MoveTo(target);
    }

    private Vector3 StartPoint(Vector3 center, float angleRad, float startSign)
    {
        Vector3 d = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        Vector3 p = center + d * (passDistance * startSign);
        p.y = center.y;
        return p;
    }

    private Vector3 EndPoint(Vector3 center, float angleRad, float startSign)
    {
        Vector3 d = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad));
        Vector3 p = center - d * (passDistance * startSign);
        p.y = center.y;
        return p;
    }

    private IEnumerator MoveLinear(TokenAgent agent, Vector3 from, Vector3 to, float seconds)
    {
        if (agent == null) yield break;

        // Если from задаётся как "текущая позиция" — не дёргаем лишний раз.
        agent.transform.position = from;

        if (seconds <= 0f)
        {
            agent.transform.position = to;
            yield break;
        }

        float t = 0f;
        while (t < seconds)
        {
            if (agent == null) yield break;
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / seconds);
            agent.transform.position = Vector3.Lerp(from, to, k);
            yield return null;
        }

        agent.transform.position = to;
    }
}
