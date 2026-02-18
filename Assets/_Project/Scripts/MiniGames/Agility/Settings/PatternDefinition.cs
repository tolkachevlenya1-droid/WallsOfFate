using UnityEngine;

[CreateAssetMenu(menuName = "MiniGame/Dex/PatternDefinition")]
public class PatternDefinition : ScriptableObject
{
    public string id = "DEX_E_01";
    public PatternTier tier = PatternTier.Easy;

    [Header("Timing")]
    [Min(0.1f)] public float duration = 3f;
    [Min(0f)] public float telegraphTime = 0.7f;
    [Min(0f)] public float cooldownAfter = 0.3f;

    [Header("Balance")]
    [Range(0f, 1.5f)] public float intensity = 0.5f;
    public PatternTag tags = PatternTag.None;
    public PatternTag forbiddenWithTags = PatternTag.None;

    [Header("Dex gating / weighting")]
    public int minDex = 1;
    public int maxDex = 10;

    [Tooltip("Вес выбора по нормализованному dex 0..1 (чем выше — тем чаще выбирается).")]
    public AnimationCurve weightByDex = AnimationCurve.Linear(0, 1f, 1, 1f);

    [Header("Prefab")]
    public PatternBehaviour patternPrefab;
}
