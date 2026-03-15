using UnityEngine;

public class MovementModifiers : MonoBehaviour
{
    // 1.0 = нормальная скорость. 0.7 = замедление.
    public float speedMultiplier { get; private set; } = 1f;

    // 1.0 = нормальный контроль. <1 = хуже контроль (скольжение/инерция).
    public float controlMultiplier { get; private set; } = 1f;

    private float _speedUntil;
    private float _controlUntil;

    public void ApplySpeedMultiplier(float mult, float seconds)
    {
        speedMultiplier = Mathf.Min(speedMultiplier, mult);
        _speedUntil = Mathf.Max(_speedUntil, Time.time + seconds);
    }

    public void ApplyControlMultiplier(float mult, float seconds)
    {
        controlMultiplier = Mathf.Min(controlMultiplier, mult);
        _controlUntil = Mathf.Max(_controlUntil, Time.time + seconds);
    }   

    private void Update()
    {
        if (Time.time > _speedUntil) speedMultiplier = 1f;
        if (Time.time > _controlUntil) controlMultiplier = 1f;
    }
}
