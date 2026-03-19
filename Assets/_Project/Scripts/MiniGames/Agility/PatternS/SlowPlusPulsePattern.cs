using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPlusPulsePattern : FormationPhasePattern
{
    [Range(0.2f, 0.8f)] public float actionRadiusFactor = 0.42f;
    public int sweepCount = 3;
    public float pauseBetweenSweeps = 0.12f;
    [Range(0.02f, 0.3f)] public float centerRadiusFactor = 0.08f;

    protected override void CreateTelegraphs()
    {
        float outerRadius = CurrentActionRadius();
        float lineLength = outerRadius * 2.15f;
        float laneWidth = Mathf.Max(0.6f, outerRadius * 0.28f);
        AddTelegraphLine(Center, new Vector3(lineLength, 0.1f, laneWidth), 0f);
        AddTelegraphLine(Center, new Vector3(laneWidth, 0.1f, lineLength), 0f);
    }

    protected override IReadOnlyList<Vector3> BuildEntryTargets()
    {
        return CardinalTargets(CurrentActionRadius());
    }

    protected override IEnumerator ExecuteFormation(float activeDuration)
    {
        Vector3[] outerTargets = CardinalTargets(CurrentActionRadius());
        Vector3[] oppositeOuterTargets = RotateTargets(outerTargets, 2);
        Vector3[] innerTargets = CardinalTargets(CurrentCenterRadius());

        var sequence = new List<IReadOnlyList<Vector3>>();
        bool useOpposite = true;
        for (int cycle = 0; cycle < Mathf.Max(1, sweepCount); cycle++)
        {
            sequence.Add(innerTargets);
            sequence.Add(useOpposite ? oppositeOuterTargets : outerTargets);
            useOpposite = !useOpposite;
        }

        int steps = sequence.Count;
        float totalPause = pauseBetweenSweeps * Mathf.Max(0, steps - 1);
        float moveDuration = Mathf.Max(0.3f, (activeDuration - totalPause) / steps);
        float timelineDuration = moveDuration * steps + totalPause;
        float timelineElapsed = 0f;

        for (int stepIndex = 0; stepIndex < steps; stepIndex++)
        {
            IReadOnlyList<Vector3> targets = sequence[stepIndex];

            float moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
            timelineElapsed += moveDuration;
            float moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
            yield return MoveActors(targets, moveDuration, 1f, moveStart, moveEnd);

            if (stepIndex < steps - 1 && pauseBetweenSweeps > 0f)
            {
                float pauseStart = moveEnd;
                timelineElapsed += pauseBetweenSweeps;
                float pauseEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
                yield return WaitForSecondsWithCoreProgress(pauseBetweenSweeps, pauseStart, pauseEnd);
            }
        }
    }

    private float CurrentActionRadius()
    {
        return Mathf.Max(1.05f, ArenaRadius * actionRadiusFactor);
    }

    private float CurrentCenterRadius()
    {
        return Mathf.Max(0.12f, ArenaRadius * centerRadiusFactor);
    }
}
