using Game;
using UnityEngine;

public class PlayChestAnimation : MonoBehaviour
{
    [SerializeField] private bool _dependFromQuests = false;
    [SerializeField] private string animationName = "Armature_Chest|Chest_Open";
    //private InteractibleItemInfluenceArea influenceAria;
    private Animator _anim;
    private bool _opened;

    private void Awake() => _anim = GetComponentInChildren<Animator>();

    //private void Start()
    //{
    //    influenceAria = this.GetComponent<InteractibleItemInfluenceArea>();

    //    influenceAria.OnItemInteracted += Triggered;
    //}

    // вызывается InteractManager-ом через TryTrigger(...)
    public void Triggered(TriggerEvent eventData) {
        if (!eventData.IsEnteracted) return;
        
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
