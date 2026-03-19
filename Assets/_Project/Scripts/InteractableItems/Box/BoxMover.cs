using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class BoxMover : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float followSharpness = 30f; 
        [SerializeField] private float maxSpeed = 10f;


        private Rigidbody rb;
        private Transform playerTransform;
        private bool isBeingHeld;
        private Vector3 moveAxis;
        private Vector3 grabOffset;
        private Collider boxCollider;
        private Collider playerCollider;

        private enum GrabAxis { ForwardBack, LeftRight }

        [Inject]
        private void Construct(PlayerMoveController playerTransform)
        {
            this.playerTransform = playerTransform.gameObject.transform;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            boxCollider = GetComponent<Collider>();
        }

        private void FixedUpdate()
        {
            if (!isBeingHeld || playerTransform == null) return;

            Vector3 playerPosXZ = new Vector3(playerTransform.position.x, 0, playerTransform.position.z);
            Vector3 targetPosXZ = playerPosXZ + grabOffset;
            Vector3 targetPos = new Vector3(targetPosXZ.x, transform.position.y, targetPosXZ.z);

            Vector3 distanceDelta = targetPos - rb.position;
            Vector3 targetVelocity = distanceDelta * followSharpness;
            targetVelocity = Vector3.ClampMagnitude(targetVelocity, maxSpeed);
            Vector3 velocityError = targetVelocity - rb.linearVelocity;
            velocityError.y = 0;

            rb.AddForce(velocityError, ForceMode.VelocityChange);

            //Vector3 moveDirection = (targetPos - transform.position).normalized;
            //float distance = Vector3.Distance(transform.position, targetPos);
            //Vector3 force = moveDirection * (distance * maxForse);
            //rb.AddForce(force, ForceMode.Force);

            //if (rb.linearVelocity.magnitude > maxSpeed)
            //{
            //    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            //}
            // Целевая позиция с сохранением исходной высоты коробки
            //rb.MovePosition(targetPos);


            // Рассчитываем силу пропорционально расстоянию

            // Применяем силу

            //Ограничиваем максимальную скорость

            //Vector3 playerDelta = playerTransform.position - transform.position;

            //float moveInput = Vector3.Dot(playerDelta.normalized, moveAxis);

            //RotateWithPlayer();
        }

        public void StartHolding()
        {
            isBeingHeld = true;
            playerCollider = playerTransform.GetComponent<Collider>();

            if (playerCollider != null && boxCollider != null)
            {
                Physics.IgnoreCollision(boxCollider, playerCollider, true);
            }

            isBeingHeld = true;

            Vector3 playerPos = playerTransform.position;
            Vector3 boxPos = transform.position;
            Vector3 dirToPlayer = (playerPos - boxPos).normalized;
            
            Vector3 playerPosXZ = new Vector3(playerPos.x, 0, playerPos.z);
            Vector3 boxPosXZ = new Vector3(boxPos.x, 0, boxPos.z);
            grabOffset = boxPosXZ - playerPosXZ;

            BoxGripPoints gripPoints = GetComponent<BoxGripPoints>();
            if (gripPoints != null && gripPoints.points.Length == 4)
            {
                Transform closestPoint = null;
                float minDist = float.MaxValue;
                foreach (Transform point in gripPoints.points)
                {
                    if (point == null) continue;
                    float dist = Vector3.Distance(playerTransform.position, point.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestPoint = point;
                    }
                }

                if (closestPoint != null)
                {
                    int index = System.Array.IndexOf(gripPoints.points, closestPoint);
                    if (index == 0 || index == 1)
                    {
                        //moveAxis = transform.forward; // ось вперёд/назад
                        moveAxis = gripPoints.points[0].position - gripPoints.points[1].position;
                    }
                    else 
                    {
                        //moveAxis = transform.right; // ось влево/вправо
                        moveAxis = gripPoints.points[2].position - gripPoints.points[3].position;
                    }
                }
                else
                {
                    // Запасной вариант
                    moveAxis = transform.forward;
                }
            }
            //else
            //{
            //    // Запасной вариант – как раньше, по смещению
            //    Vector3 playerPosXZ = new Vector3(playerPos.x, 0, playerPos.z);
            //    Vector3 boxPosXZ = new Vector3(boxPos.x, 0, boxPos.z);
            //    grabOffset = boxPosXZ - playerPosXZ;
            //}

            playerTransform.GetComponent<PlayerMoveController>()?.InBoxGrabMode(moveAxis.normalized);

            rb.freezeRotation = true;
        }

        public void StopHolding()
        {
            isBeingHeld = false;
            if (playerCollider != null && boxCollider != null)
            {
                Physics.IgnoreCollision(boxCollider, playerCollider, false);
            }

            isBeingHeld = false;
            rb.freezeRotation = false;
            playerTransform.GetComponent<PlayerMoveController>()?.StopBoxGrabMode();
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