using System.Collections;
using UnityEngine;

public abstract class PatternBehaviour : MonoBehaviour
{
    public PatternDefinition Definition { get; private set; }
    protected PatternContext Ctx { get; private set; }

    public void Init(PatternDefinition def, PatternContext ctx)
    {
        Definition = def;
        Ctx = ctx;
    }

    // ¬озвращает корутину, которую будет исполн€ть Sequencer
    public abstract IEnumerator Run();
}
