using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SurfaceHazardZone : MonoBehaviour
{
    public enum ZoneEffect
    {
        Slow,
        Slip,
        Burn
    }

    [SerializeField] private float lifetime = 2.5f;
    [SerializeField] private float speedMultiplier = 0.65f;
    [SerializeField] private float controlMultiplier = 0.65f;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private int burnDamage = 1;

    private readonly Dictionary<PlayerHealth, float> _nextBurnTick = new();
    private ZoneEffect _zoneEffect;

    public void Configure(ZoneEffect effect, float seconds, float speedMult = 0.65f, float controlMult = 0.65f)
    {
        _zoneEffect = effect;
        lifetime = Mathf.Max(0.1f, seconds);
        speedMultiplier = speedMult;
        controlMultiplier = controlMult;
    }

    private void Awake()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        var hp = other.GetComponentInParent<PlayerHealth>();
        var mods = other.GetComponentInParent<MovementModifiers>();
        if (hp == null && mods == null)
            return;

        switch (_zoneEffect)
        {
            case ZoneEffect.Slow:
                if (mods != null)
                    mods.ApplySpeedMultiplier(speedMultiplier, 0.15f);
                break;
            case ZoneEffect.Slip:
                if (mods != null)
                    mods.ApplyControlMultiplier(controlMultiplier, 0.15f);
                break;
            case ZoneEffect.Burn:
                if (hp == null)
                    return;

                if (!_nextBurnTick.TryGetValue(hp, out float nextTick))
                    nextTick = 0f;

                if (Time.time >= nextTick)
                {
                    hp.TakeDamage(burnDamage);
                    _nextBurnTick[hp] = Time.time + damageInterval;
                }
                break;
        }
    }
}
