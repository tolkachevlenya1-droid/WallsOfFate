using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MiniGame/Dex/PatternDatabase")]
public class PatternDatabase : ScriptableObject
{
    public List<PatternDefinition> patterns = new();

    private readonly List<PatternDefinition> _runtimePatterns = new();

    public IEnumerable<PatternDefinition> All => patterns.Concat(_runtimePatterns).Where(p => p != null);

    public void ReplaceRuntimePatterns(IEnumerable<PatternDefinition> runtimePatterns)
    {
        _runtimePatterns.Clear();
        if (runtimePatterns == null)
            return;

        _runtimePatterns.AddRange(runtimePatterns.Where(p => p != null));
    }

    public void ClearRuntimePatterns()
    {
        _runtimePatterns.Clear();
    }
}
