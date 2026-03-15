using UnityEngine;

[CreateAssetMenu(menuName = "MiniGame/Dex/MiniGameConfig")]
public class MiniGameConfig : ScriptableObject
{
    [Header("Run")]
    [Min(1f)] public float runDuration = 35f;
    [Min(0f)] public float countdownSeconds = 3f;
    [Min(0f)] public float safePhaseSeconds = 2.5f;

    [Header("Player")]
    [Min(1)] public int startingHp = 3;
    [Min(0f)] public float iFramesSeconds = 0.8f;

    [Header("Slots")]
    [Tooltip("Длина одного слота, из которых собирается RunPlan.")]
    [Min(1f)] public float slotSeconds = 5f;
}
