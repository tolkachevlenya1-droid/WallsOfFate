using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 6f;
    public float acceleration = 25f;
    public float deceleration = 35f;

    private Rigidbody _rb;
    private MovementModifiers _mods;

    private Vector3 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mods = GetComponent<MovementModifiers>();
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        _input = new Vector3(x, 0f, z).normalized;
    }

    private void FixedUpdate()
    {
        float speedMult = _mods ? _mods.speedMultiplier : 1f;
        float controlMult = _mods ? _mods.controlMultiplier : 1f;

        float targetSpeed = maxSpeed * speedMult;
        Vector3 desiredVel = _input * targetSpeed;

        Vector3 currentVel = _rb.linearVelocity;
        Vector3 delta = desiredVel - currentVel;

        float accel = (_input.sqrMagnitude > 0.001f) ? acceleration : deceleration;
        accel *= controlMult;

        Vector3 change = Vector3.ClampMagnitude(delta, accel * Time.fixedDeltaTime);
        _rb.linearVelocity = currentVel + change;
    }
}
