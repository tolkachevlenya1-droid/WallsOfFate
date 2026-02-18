using UnityEngine;

[CreateAssetMenu(menuName = "MiniGame/Dex/DexDifficultyProfile")]
public class DexDifficultyProfile : ScriptableObject
{
    [Header("Dex Range")]
    public int minDex = 1;
    public int maxDex = 10;

    [Header("Tier Mix (by normalized dex 0..1)")]
    [Tooltip("Доля Easy (0..1).")]
    public AnimationCurve easyShare = AnimationCurve.Linear(0, 0.8f, 1, 0.3f);
    [Tooltip("Доля Medium (0..1).")]
    public AnimationCurve mediumShare = AnimationCurve.Linear(0, 0.2f, 1, 0.45f);
    [Tooltip("Доля Hard (0..1).")]
    public AnimationCurve hardShare = AnimationCurve.Linear(0, 0.0f, 1, 0.25f);

    [Header("Density / Pacing")]
    [Tooltip("Бюджет плотности/одновременности. Чем выше — тем больше/плотнее связки.")]
    public AnimationCurve maxIntensityBudget = AnimationCurve.Linear(0, 0.85f, 1, 1.15f);

    [Tooltip("Шанс сделать связку (2 паттерна в слот), если позволяет бюджет.")]
    public AnimationCurve comboChance = AnimationCurve.Linear(0, 0.0f, 1, 0.25f);

    [Tooltip("Частота окон отдыха после Medium/Hard (0..1). 1 = почти всегда отдых.")]
    public AnimationCurve restFrequency = AnimationCurve.Linear(0, 0.9f, 1, 0.4f);

    public float NormalizeDex(int dex)
    {
        dex = Mathf.Clamp(dex, minDex, maxDex);
        return (dex - minDex) / (float)(maxDex - minDex);
    }
}
