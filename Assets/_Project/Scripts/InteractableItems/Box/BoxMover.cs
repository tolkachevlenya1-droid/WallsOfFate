using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoxMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;

        private Rigidbody rb;
        private Transform playerTransform;
        private bool isBeingHeld;
        private Vector3 moveAxis;

        private enum GrabAxis { ForwardBack, LeftRight }

        [Inject]
        private void Construct(Transform playerTransform)
        {
            this.playerTransform = playerTransform;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = true;
        }

        private void FixedUpdate()
        {
            if (!isBeingHeld || playerTransform == null) return;

            Vector3 playerDelta = playerTransform.position - transform.position;

            float moveInput = Vector3.Dot(playerDelta.normalized, moveAxis);

            Vector3 targetPos = transform.position + moveAxis * moveInput * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);

            RotateWithPlayer();
        }

        public void StartHolding()
        {
            isBeingHeld = true;

            GrabAxis grabAxis = Mathf.Abs(playerTransform.localPosition.x) > Mathf.Abs(playerTransform.localPosition.z)
                ? GrabAxis.LeftRight
                : GrabAxis.ForwardBack;

            if (grabAxis == GrabAxis.ForwardBack)
                moveAxis = transform.forward;
            else
                moveAxis = transform.right;

            rb.freezeRotation = true;
        }

        public void StopHolding()
        {
            isBeingHeld = false;
            rb.freezeRotation = false;
        }

        private void RotateWithPlayer()
        {
            Vector3 boxRotation = transform.eulerAngles;
            boxRotation.y = playerTransform.eulerAngles.y;
            rb.MoveRotation(Quaternion.Euler(boxRotation));
        }

        public bool IsBeingHeld => isBeingHeld;
    }
}