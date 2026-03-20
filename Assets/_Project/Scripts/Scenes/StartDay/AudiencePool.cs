using UnityEngine;
using System.Collections.Generic;

public class AudiencePool : MonoBehaviour
{
    public static AudiencePool Instance { get; private set; }

    [Tooltip("Полный список из 9 ScriptableObjects NPCDefinition")]
    [SerializeField] private List<NPCDefinition> _allNpcs;

    private readonly List<NPCDefinition> _remaining = new();
    public IReadOnlyList<NPCDefinition> Remaining => _remaining;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetPool();
    }

    /* ---------- главное нововведение ---------- */
    public void ResetPool()
    {
        _remaining.Clear();
        _remaining.AddRange(_allNpcs);
    }
    /* ----------------------------------------- */

    public bool TryTakeRandom(out NPCDefinition npc)
    {
        if (_remaining.Count == 0) { npc = null; return false; }

        int idx = Random.Range(0, _remaining.Count);
        npc = _remaining[idx];
        _remaining.RemoveAt(idx);
        return true;
    }
}
