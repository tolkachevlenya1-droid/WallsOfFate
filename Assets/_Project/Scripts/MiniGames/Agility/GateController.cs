using System.Collections;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public Animator animator;
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    public float openDelay = 0.2f;
    public float closeDelay = 0.2f;

    public IEnumerator Open()
    {
        if (animator != null) animator.SetTrigger(openTrigger);
        if (openDelay > 0f) yield return new WaitForSeconds(openDelay);
    }

    public IEnumerator Close()
    {
        if (animator != null) animator.SetTrigger(closeTrigger);
        if (closeDelay > 0f) yield return new WaitForSeconds(closeDelay);
    }
}
