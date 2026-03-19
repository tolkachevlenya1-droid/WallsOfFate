using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossThenPlusPattern : FormationPhasePattern
{
    [Range(0.2f, 0.8f)] public float outerRadiusFactor = 0.46f;
    [Range(0.02f, 0.3f)] public float centerRadiusFactor = 0.06f;
    public float pauseBetweenPhases = 0.08f;
    public bool rotateClockwise = true;

    protected override void CreateTelegraphs()
    {
        float outerRadius = CurrentOuterRadius();
        float lineLength = outerRadius * 2.15f;
        float laneWidth = Mathf.Max(0.6f, outerRadius * 0.28f);
        Vector3 telegraphSize = new Vector3(lineLength, 0.1f, laneWidth);

        AddTelegraphLine(Center, telegraphSize, 45f);
        AddTelegraphLine(Center, telegraphSize, -45f);
        AddTelegraphLine(Center, telegraphSize, 0f);
        AddTelegraphLine(Center, telegraphSize, 90f);
    }

    protected override IReadOnlyList<Vector3> BuildEntryTargets()
    {
        return DiagonalTargets(CurrentOuterRadius());
    }

    protected override IEnumerator ExecuteFormation(float activeDuration)
    {
        Vector3[] outerDiagonal = DiagonalTargets(CurrentOuterRadius());
        Vector3[] innerDiagonal = DiagonalTargets(CurrentCenterRadius());
        Vector3[] oppositeOuterDiagonal = RotateTargets(outerDiagonal, 2);

        float[] startArcAngles = AnglesFrom(oppositeOuterDiagonal);
        float[] endArcAngles = rotateClockwise
            ? new[] { -90f, 180f, 0f, 90f }
            : new[] { 0f, -90f, 90f, 180f };

        Vector3[] outerCardinal = TargetsFromAngles(CurrentOuterRadius(), endArcAngles);
        Vector3[] innerCardinal = TargetsFromAngles(CurrentCenterRadius(), endArcAngles);
        Vector3[] oppositeOuterCardinal = RotateTargets(outerCardinal, 2);

        int moveCount = 5;
        float totalPause = pauseBetweenPhases * Mathf.Max(0, moveCount - 1);
        float moveDuration = Mathf.Max(0.28f, (activeDuration - totalPause) / moveCount);
        float timelineDuration = moveCount * moveDuration + totalPause;
        float timelineElapsed = 0f;

        float moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
        timelineElapsed += moveDuration;
        float moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
        yield return MoveActors(innerDiagonal, moveDuration, 1f, moveStart, moveEnd);
        if (pauseBetweenPhases > 0f)
        {
            float pauseStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
            timelineElapsed += pauseBetweenPhases;
            float pauseEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
            yield return WaitForSecondsWithCoreProgress(pauseBetweenPhases, pauseStart, pauseEnd);
        }

        moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
        timelineElapsed += moveDuration;
        moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
        yield return MoveActors(oppositeOuterDiagonal, moveDuration, 1f, moveStart, moveEnd);
        if (pauseBetweenPhases > 0f)
        {
            float pauseStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
            timelineElapsed += pauseBetweenPhases;
            float pauseEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
            yield return WaitForSecondsWithCoreProgress(pauseBetweenPhases, pauseStart, pauseEnd);
        }

        moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
        timelineElapsed += moveDuration;
        moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
        yield return MoveActorsAlongArc(startArcAngles, endArcAngles, CurrentOuterRadius(), moveDuration, 1f, moveStart, moveEnd);
        if (pauseBetweenPhases > 0f)
        {
            float pauseStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
            timelineElapsed += pauseBetweenPhases;
            float pauseEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
            yield return WaitForSecondsWithCoreProgress(pauseBetweenPhases, pauseStart, pauseEnd);
        }

        moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
        timelineElapsed += moveDuration;
        moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
        yield return MoveActors(innerCardinal, moveDuration, 1f, moveStart, moveEnd);
        if (pauseBetweenPhases > 0f)
        {
            float pauseStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
            timelineElapsed += pauseBetweenPhases;
            float pauseEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
            yield return WaitForSecondsWithCoreProgress(pauseBetweenPhases, pauseStart, pauseEnd);
        }

        moveStart = timelineDuration > 0f ? timelineElapsed / timelineDuration : 0f;
        timelineElapsed += moveDuration;
        moveEnd = timelineDuration > 0f ? timelineElapsed / timelineDuration : 1f;
        yield return MoveActors(oppositeOuterCardinal, moveDuration, 1f, moveStart, moveEnd);
    }

    private Vector3[] TargetsFromAngles(float radius, IReadOnlyList<float> angles)
    {
        var targets = new Vector3[angles.Count];
        for (int i = 0; i < angles.Count; i++)
            targets[i] = PointOnRing(angles[i], radius);

        return targets;
    }

    private float[] AnglesFrom(IReadOnlyList<Vector3> targets)
    {
        var angles = new float[targets.Count];
        for (int i = 0; i < targets.Count; i++)
            angles[i] = AngleFromCenter(targets[i]);

        return angles;
    }

    private float CurrentOuterRadius()
    {
        return Mathf.Max(1.05f, ArenaRadius * outerRadiusFactor);
    }

    private float CurrentCenterRadius()
    {
        return Mathf.Max(0.12f, ArenaRadius * centerRadiusFactor);
    }
}
