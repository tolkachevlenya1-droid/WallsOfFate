using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossFormationPattern : FormationPhasePattern
{
    public float diagonalRadius = 1.8f;
    public float crossRadius = 0.32f;

    protected override void CreateTelegraphs()
    {
        float lineLength = Mathf.Max(diagonalRadius * 2.25f, ArenaRadius * 1.1f);
        AddTelegraphLine(Center, new Vector3(lineLength, 0.08f, 0.34f), 45f);
        AddTelegraphLine(Center, new Vector3(lineLength, 0.08f, 0.34f), -45f);
    }

    protected override IReadOnlyList<Vector3> BuildEntryTargets()
    {
        return DiagonalTargets(Mathf.Min(diagonalRadius, ArenaRadius * 0.58f));
    }

    protected override IEnumerator ExecuteFormation(float activeDuration)
    {
        float outer = Mathf.Min(diagonalRadius, ArenaRadius * 0.58f);
        int swaps = Definition.intensity >= 0.82f ? 2 : 1;
        float moveDuration = Mathf.Max(0.3f, activeDuration * 0.24f);
        float holdDuration = Mathf.Max(0.18f, (activeDuration - moveDuration * swaps * 2f) / (swaps * 2f + 1f));

        Vector3[] outerTargets = DiagonalTargets(outer);
        Vector3[] oppositeTargets = new[] { outerTargets[3], outerTargets[2], outerTargets[1], outerTargets[0] };

        yield return new WaitForSeconds(holdDuration);

        for (int i = 0; i < swaps; i++)
        {
            yield return MoveActors(oppositeTargets, moveDuration);
            yield return new WaitForSeconds(holdDuration);
            yield return MoveActors(outerTargets, moveDuration);
            yield return new WaitForSeconds(holdDuration);
        }
    }
}
