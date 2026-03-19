using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitFormationPattern : FormationPhasePattern
{
    public float orbitRadius = 1.95f;

    protected override void CreateTelegraphs()
    {
        Vector3[] targets = DiagonalTargets(Mathf.Min(orbitRadius, ArenaRadius * 0.62f));
        for (int i = 0; i < targets.Length; i++)
            AddTelegraphZone(targets[i], 0.34f);
    }

    protected override IReadOnlyList<Vector3> BuildEntryTargets()
    {
        return DiagonalTargets(Mathf.Min(orbitRadius, ArenaRadius * 0.62f));
    }

    protected override IEnumerator ExecuteFormation(float activeDuration)
    {
        Vector3[] ringTargets = DiagonalTargets(Mathf.Min(orbitRadius, ArenaRadius * 0.62f));
        int turns = Definition.intensity >= 0.82f ? 2 : 1;
        float moveDuration = Mathf.Max(0.3f, activeDuration * 0.24f);
        float holdDuration = Mathf.Max(0.2f, (activeDuration - moveDuration * turns) / (turns + 1f));

        yield return new WaitForSeconds(holdDuration);

        for (int i = 0; i < turns; i++)
        {
            yield return MoveActors(RotateTargets(ringTargets, i + 1), moveDuration);
            yield return new WaitForSeconds(holdDuration);
        }
    }
}
