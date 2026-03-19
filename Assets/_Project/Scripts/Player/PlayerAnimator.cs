using UnityEngine;

/// <summary>
/// Handles animation parameters for the player character and plays footstep sounds.
/// The main goal is to keep the locomotion blend-tree perfectly smooth, even when the
/// physics update rate fluctuates or the character changes state (pushing / running / idle).
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    #region Inspector
    [Header("Movement")]
    [Tooltip("Top‑end speed of the character in m/s. Used for normalising the blend‑tree parameter.")]
    [SerializeField] private float maxSpeed = 4.5f;

    [Tooltip("Time it takes for the Speed parameter to reach the target value (seconds).")]
    [SerializeField] private float speedSmoothTime = 0.1f;

    [Header("Pushing Box")]
    [Tooltip("Tag that marks objects the player can push.")]
    [SerializeField] private string boxTag = "Box";
    [SerializeField] private float pushAnimationSpeed = 1.5f;

    [Tooltip("Minimal speed (m/s) at which the pushing animation starts to blend in.")]
    [SerializeField] private float pushMoveThreshold = 0.05f;

    [Header("Footsteps (optional)")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] walkStepClips;
    [SerializeField] private AudioClip[] runStepClips;
    [SerializeField] private float baseFootstepInterval = 0.5f; // walk cadence
    [SerializeField] private float runFootstepInterval = 0.3f; // run cadence
    #endregion

    private Animator animator;
    private Vector3 lastPos;
    private float currentSpeedParam; // smoothed animator parameter [0‑1]
    private float speedRef;          // velocity reference for SmoothDamp
    private float footstepTimer;

    // ──────────────────────────────────────────────────────────────────────────────
    #region Unity
    private void Awake()
    {
        animator = GetComponent<Animator>();
        lastPos = transform.position;
    }

    private void Update()
    {
        UpdateLocomotion();   // handles speed, pushing, animator parameters
        UpdateFootsteps();    // handles audio cadence
        lastPos = transform.position; // cache for next frame after all logic
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Locomotion
    private void UpdateLocomotion()
    {
        // 1) вычисляем текущую скорость и нормализуем
        float rawSpeed = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        float targetNorm = Mathf.Clamp01(rawSpeed / maxSpeed);
        currentSpeedParam = Mathf.SmoothDamp(currentSpeedParam, targetNorm, ref speedRef, speedSmoothTime);

        // 2) определяем состояние толкания
        bool isPushing = transform.parent != null
                         && transform.parent.CompareTag(boxTag);

        // 3) вычисляем параметр PushSpeed (0 или 1)
        float pushSpeedParam = isPushing
            ? (currentSpeedParam >= pushMoveThreshold ? 1f : 0f)
            : 0f;

        // 4) отправляем параметры в аниматор
        animator.SetBool("IsPushing", isPushing);
        animator.SetFloat("PushSpeed", pushSpeedParam);

        // 5) Останавливаем анимацию, если толкаем, но скорость нулевая
        if (isPushing && pushSpeedParam == 0f)
        {
            animator.speed = 0f;
        }
        else
        {
            // во всех остальных случаях включаем аниматор и, если не толкаем,
            // обновляем параметр Speed для blend-tree передвижения
            animator.speed = pushAnimationSpeed;

            if (!isPushing)
            {
                animator.SetFloat("Speed", currentSpeedParam);
                animator.SetBool("IsPushing", false);
            }
        }
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Footstep Audio
    private void UpdateFootsteps()
    {
        if (footstepSource == null) return; // sound system is optional

        if (currentSpeedParam > 0.05f)
        {
            footstepTimer += Time.deltaTime;
            float interval = Mathf.Lerp(baseFootstepInterval, runFootstepInterval, currentSpeedParam);

            if (footstepTimer >= interval)
            {
                PlayRandomFootstep(currentSpeedParam > 0.6f);
                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f; // reset so the first step triggers instantly when we start moving again
        }
    }

    private void PlayRandomFootstep(bool running)
    {
        AudioClip[] bank = running ? runStepClips : walkStepClips;
        if (bank == null || bank.Length == 0) return;

        footstepSource.clip = bank[Random.Range(0, bank.Length)];
        footstepSource.Play();
    }
    #endregion

    // ──────────────────────────────────────────────────────────────────────────────
    #region Public Triggers
    public void PlayPickupFloor() => animator.SetTrigger("PickupFloor");
    public void PlayPickupBody() => animator.SetTrigger("PickupBody");
    public void PlayOpenChest() => animator.SetTrigger("OpenChest");
    public void PlayGrabBox() => animator.SetTrigger("GrabBox");
    #endregion
}
