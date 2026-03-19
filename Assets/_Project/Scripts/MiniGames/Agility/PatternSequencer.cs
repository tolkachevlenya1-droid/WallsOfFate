using System.Collections;
using UnityEngine;

public class PatternSequencer : MonoBehaviour
{
    public Transform arenaCenter;
    public EntrancePoints entrances;

    private PatternContext _ctx;
    private PatternBehaviour _activePattern;
    private int _currentPatternIndex;
    private int _totalPatterns;

    public event System.Action<int, int, float> OnPatternCoreProgress;
    public event System.Action<int, int> OnPatternCoreCompleted;

    public void Init(PlayerHealth hp, MovementModifiers mods)
    {
        if (arenaCenter == null)
            arenaCenter = AgilitySceneUtility.FindTransform("Board");
        if (entrances == null)
            entrances = AgilitySceneUtility.FindInLoadedScene<EntrancePoints>();

        _ctx = new PatternContext
        {
            arenaCenter = arenaCenter,
            entrances = entrances,
            playerHealth = hp,
            playerModifiers = mods,
            runner = this
        };

        _ctx.boardY = _ctx.arenaCenter != null ? _ctx.arenaCenter.position.y : 0f;
        _ctx.hazardHeightOffset = hp != null ? Mathf.Max(0f, hp.transform.position.y - _ctx.boardY) : 0.5f;
    }

    public IEnumerator Play(RunPlan plan)
    {
        if (_ctx == null || _ctx.playerHealth == null)
            yield break;

        float runStartedAt = Time.time;
        _totalPatterns = plan?.items?.Count ?? 0;

        for (int patternIndex = 0; patternIndex < plan.items.Count; patternIndex++)
        {
            var item = plan.items[patternIndex];
            if (_ctx.playerHealth.IsDead) yield break;

            while (Time.time - runStartedAt < item.startTime)
            {
                if (_ctx.playerHealth.IsDead)
                    yield break;

                yield return null;
            }

            var def = item.pattern;
            _currentPatternIndex = patternIndex;
            if (def.patternPrefab == null)
            {
                Debug.LogWarning($"Pattern {def.id} has no prefab.");
                yield return WaitUntil(runStartedAt, item.endTime);
                continue;
            }

            var obj = Instantiate(def.patternPrefab, transform);
            obj.gameObject.SetActive(true);
            obj.Init(def, _ctx);
            obj.BeginTelegraph();
            _activePattern = obj;

            if (def.telegraphTime > 0f)
                yield return WaitForSecondsOrDeath(def.telegraphTime);
            if (_ctx.playerHealth.IsDead)
            {
                obj.Cleanup();
                if (obj != null)
                    Destroy(obj.gameObject);
                _activePattern = null;
                yield break;
            }

            yield return obj.Run();
            obj.Cleanup();

            if (obj != null)
                Destroy(obj.gameObject);

            if (_activePattern == obj)
                _activePattern = null;

            yield return WaitUntil(runStartedAt, item.endTime);
        }
    }

    public void StopActivePattern()
    {
        if (_activePattern == null)
            return;

        _activePattern.Cleanup();
        Destroy(_activePattern.gameObject);
        _activePattern = null;
    }

    public void NotifyPatternCoreStarted()
    {
        OnPatternCoreProgress?.Invoke(_currentPatternIndex, _totalPatterns, 0f);
    }

    public void NotifyPatternCoreProgress(float normalized)
    {
        OnPatternCoreProgress?.Invoke(_currentPatternIndex, _totalPatterns, Mathf.Clamp01(normalized));
    }

    public void NotifyPatternCoreCompleted()
    {
        OnPatternCoreProgress?.Invoke(_currentPatternIndex, _totalPatterns, 1f);
        OnPatternCoreCompleted?.Invoke(_currentPatternIndex, _totalPatterns);
    }

    private IEnumerator WaitUntil(float runStartedAt, float targetTime)
    {
        while (Time.time - runStartedAt < targetTime)
        {
            if (_ctx.playerHealth.IsDead)
                yield break;

            yield return null;
        }
    }

    private IEnumerator WaitForSecondsOrDeath(float seconds)
    {
        float remaining = seconds;
        while (remaining > 0f)
        {
            if (_ctx.playerHealth.IsDead)
                yield break;

            remaining -= Time.deltaTime;
            yield return null;
        }
    }
}
