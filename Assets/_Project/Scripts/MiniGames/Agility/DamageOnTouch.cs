using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageOnTouch : MonoBehaviour
{
    public int damage = 1;

    [Tooltip("Если true — ожидаем, что коллайдер уронщика Trigger.")]
    public bool requireTrigger = true;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (requireTrigger && !GetComponent<Collider>().isTrigger) return;

        var hp = other.GetComponentInParent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(damage);
    }
}
