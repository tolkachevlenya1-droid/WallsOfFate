using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MiniGame/Dex/PatternDatabase")]
public class PatternDatabase : ScriptableObject
{
    public List<PatternDefinition> patterns = new();

    public IEnumerable<PatternDefinition> All => patterns;
}
