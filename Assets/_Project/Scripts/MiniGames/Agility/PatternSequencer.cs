using System.Collections;
using UnityEngine;

public class PatternSequencer : MonoBehaviour
{
    public Transform arenaCenter;
    public EntrancePoints entrances;

    private PatternContext _ctx;

    public void Init(PlayerHealth hp, MovementModifiers mods)
    {
        _ctx = new PatternContext
        {
            arenaCenter = arenaCenter,
            entrances = entrances,
            playerHealth = hp,
            playerModifiers = mods,
            runner = this
        };
    }

    public IEnumerator Play(RunPlan plan)
    {
        foreach (var item in plan.items)
        {
            if (_ctx.playerHealth.IsDead) yield break;
            var def = item.pattern;
            if (def.patternPrefab == null)
            {
                Debug.LogWarning($"Pattern {def.id} has no prefab.");
                yield return new WaitForSeconds(def.duration + def.cooldownAfter);
                continue;
            }

            var obj = Instantiate(def.patternPrefab, transform);
            obj.Init(def, _ctx);

            if (def.telegraphTime > 0f)
                yield return new WaitForSeconds(def.telegraphTime);

            yield return obj.Run();

            Destroy(obj.gameObject);
            if (def.cooldownAfter > 0f)
                yield return new WaitForSeconds(def.cooldownAfter);
        }
    }
}
