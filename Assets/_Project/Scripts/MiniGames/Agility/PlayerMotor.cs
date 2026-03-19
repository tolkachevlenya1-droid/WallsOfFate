using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 6f;
    public float acceleration = 25f;
    public float deceleration = 35f;

    [Header("Arena")]
    public bool clampToArena = true;
    public float arenaPadding = 0.45f;
    public Transform arenaCenterOverride;
    public float arenaRadiusOverride = 0f;

    [Header("Pushback")]
    public float externalVelocityDecay = 12f;
    public float maxExternalVelocity = 3.5f;

    [Header("Facing")]
    public float rotationSpeed = 720f;
    public float facingAngleOffset = 0f;

    [Header("Runtime Tuning")]
    public float runtimeSpeedMultiplier = 1f;
    public float runtimeAccelerationMultiplier = 1f;
    public float runtimeDecelerationMultiplier = 1f;

    private Rigidbody _rb;
    private MovementModifiers _mods;
    private Vector3 _input;
    private Vector3 _arenaCenter;
    private float _arenaRadius;
    private Vector3 _externalVelocity;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _mods = GetComponent<MovementModifiers>();
        RefreshArenaBounds();
    }

    private void Update()
    {
        _input = ReadMoveInput();
    }

    private void FixedUpdate()
    {
        float speedMult = _mods ? _mods.speedMultiplier : 1f;
        float controlMult = _mods ? _mods.controlMultiplier : 1f;

        float targetSpeed = maxSpeed * speedMult * runtimeSpeedMultiplier;
        Vector3 desiredVel = _input * targetSpeed;

        Vector3 currentVel = _rb.linearVelocity;
        currentVel.y = 0f;
        Vector3 delta = desiredVel - currentVel;

        float accel = (_input.sqrMagnitude > 0.001f) ? acceleration : deceleration;
        accel *= controlMult;
        accel *= _input.sqrMagnitude > 0.001f ? runtimeAccelerationMultiplier : runtimeDecelerationMultiplier;

        Vector3 change = Vector3.ClampMagnitude(delta, accel * Time.fixedDeltaTime);
        Vector3 nextVelocity = currentVel + change;
        nextVelocity += _externalVelocity;
        nextVelocity = Vector3.ClampMagnitude(nextVelocity, targetSpeed + maxExternalVelocity);
        nextVelocity.y = 0f;
        _rb.linearVelocity = nextVelocity;

        _externalVelocity = Vector3.MoveTowards(_externalVelocity, Vector3.zero, externalVelocityDecay * Time.fixedDeltaTime);

        RotateTowardsMovement(nextVelocity);

        if (clampToArena)
        {
            RefreshArenaBounds();
            Vector3 clampedPosition = AgilitySceneUtility.ClampToArena(transform.position, _arenaCenter, _arenaRadius, arenaPadding);
            if ((clampedPosition - transform.position).sqrMagnitude > 0.0001f)
            {
                _rb.position = clampedPosition;

                Vector3 planarNormal = clampedPosition - _arenaCenter;
                planarNormal.y = 0f;
                if (planarNormal.sqrMagnitude > 0.0001f)
                {
                    Vector3 outwardVelocity = Vector3.Project(_rb.linearVelocity, planarNormal.normalized);
                    if (Vector3.Dot(outwardVelocity, planarNormal) > 0f)
                        _rb.linearVelocity -= outwardVelocity;
                }
            }
        }

    }

    private Vector3 ReadMoveInput()
    {
        Vector2 move = Vector2.zero;

        var inputManager = InputManager.GetInstance();
        if (inputManager != null)
        {
            Vector3 sharedMove = inputManager.GetMoveDirection();
            move = new Vector2(sharedMove.x, sharedMove.y);
        }

#if ENABLE_INPUT_SYSTEM
        if (move.sqrMagnitude <= 0.0001f)
            move = ReadInputSystemMove();
#endif

        if (move.sqrMagnitude <= 0.0001f)
            move = ReadLegacyMove();

        return new Vector3(move.x, 0f, move.y).normalized;
    }

#if ENABLE_INPUT_SYSTEM
    private Vector2 ReadInputSystemMove()
    {
        Vector2 move = Vector2.zero;

        var gamepad = Gamepad.current;
        if (gamepad != null)
            move = gamepad.leftStick.ReadValue();

        if (move.sqrMagnitude > 0.0001f)
            return Vector2.ClampMagnitude(move, 1f);

        var keyboard = Keyboard.current;
        if (keyboard == null)
            return Vector2.zero;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            move.x -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            move.x += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            move.y -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            move.y += 1f;

        return Vector2.ClampMagnitude(move, 1f);
    }
#endif

    private Vector2 ReadLegacyMove()
    {
        try
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        catch
        {
            return Vector2.zero;
        }
    }

    private void RotateTowardsMovement(Vector3 planarVelocity)
    {
        planarVelocity.y = 0f;
        if (planarVelocity.sqrMagnitude <= 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(planarVelocity.normalized, Vector3.up)
                                    * Quaternion.Euler(0f, facingAngleOffset, 0f);
        Quaternion nextRotation = Quaternion.RotateTowards(_rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        _rb.MoveRotation(nextRotation);
    }

    private void RefreshArenaBounds()
    {
        _arenaCenter = arenaCenterOverride != null
            ? arenaCenterOverride.position
            : AgilitySceneUtility.ResolveArenaCenter();

        _arenaRadius = arenaRadiusOverride > 0f
            ? arenaRadiusOverride
            : AgilitySceneUtility.ResolveArenaRadius(arenaCenterOverride);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = arenaCenterOverride != null
            ? arenaCenterOverride.position
            : AgilitySceneUtility.ResolveArenaCenter();

        float radius = arenaRadiusOverride > 0f
            ? arenaRadiusOverride
            : AgilitySceneUtility.ResolveArenaRadius(arenaCenterOverride);

        if (radius <= 0.01f)
            return;

        Gizmos.color = new Color(1f, 0.67f, 0.12f, 0.9f);
        DrawWireCircle(center, radius);

        if (arenaPadding > 0f && radius - arenaPadding > 0.01f)
        {
            Gizmos.color = new Color(1f, 0.2f, 0.12f, 0.9f);
            DrawWireCircle(center, radius - arenaPadding);
        }
    }

    private static void DrawWireCircle(Vector3 center, float radius, int segments = 48)
    {
        Vector3 prev = center + new Vector3(radius, 0f, 0f);
        for (int i = 1; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2f;
            Vector3 next = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    public void AddExternalVelocity(Vector3 deltaVelocity)
    {
        deltaVelocity.y = 0f;
        _externalVelocity += deltaVelocity;
        _externalVelocity = Vector3.ClampMagnitude(_externalVelocity, maxExternalVelocity);
    }

    public void SetRuntimeTuning(float speedMultiplier, float accelerationMultiplier = 1f, float decelerationMultiplier = 1f)
    {
        runtimeSpeedMultiplier = Mathf.Max(0.1f, speedMultiplier);
        runtimeAccelerationMultiplier = Mathf.Max(0.1f, accelerationMultiplier);
        runtimeDecelerationMultiplier = Mathf.Max(0.1f, decelerationMultiplier);
    }

}
