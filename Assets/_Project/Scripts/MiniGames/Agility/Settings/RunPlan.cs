using System.Collections.Generic;

public class RunPlanItem
{
    public PatternDefinition pattern;
    public float startTime;
    public float endTime;
}

public class RunPlan
{
    public readonly List<RunPlanItem> items = new();
}
