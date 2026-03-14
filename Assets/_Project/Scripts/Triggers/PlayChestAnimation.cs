using UnityEngine;

public class PlayChestAnimation : MonoBehaviour, ITriggerable   // ключевая строка
{
    [SerializeField] private bool _dependFromQuests = false;
    [SerializeField] private string animationName = "Armature_Chest|Chest_Open";
    private Animator _anim;
    private bool _opened;

    private void Awake() => _anim = GetComponentInChildren<Animator>();

    // вызывается InteractManager-ом через TryTrigger(...)
    public void Triggered() {
        CompositeTrigger compositeTrigger = this.gameObject.GetComponent<CompositeTrigger>();
        if (_opened) return;
        else {
            if (_dependFromQuests) {
                if (!compositeTrigger.IsDone) return;
            }
        }
        _opened = true;
        if (_anim) _anim.Play(animationName);
    }
}
