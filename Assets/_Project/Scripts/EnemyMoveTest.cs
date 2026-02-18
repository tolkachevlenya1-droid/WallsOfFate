using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{

    public class EnemyMoveTest : MonoBehaviour
    {
        [Header("Movement Params")]

        [SerializeField] private float _runDefaultSpeed = 6.0f;
        [SerializeField] private float _runSpeed = 6.0f;
        [SerializeField] private float _rotationSpeed = 20f;
        [SerializeField] private Transform _cameraTrnsform;

        private Rigidbody _rb;
        private MiniGamePlayer _playerChar;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            _rb.useGravity = false;
        }

        private void FixedUpdate()
        {
            HandleHorizontalMovement();
        }

        public void ChangeSpeed(float speed)
        {
            _runSpeed = _runDefaultSpeed * speed;
        }

        private void HandleHorizontalMovement()
        {
            Vector2 moveInput = Vector2.zero;

            // Handling explicit key inputs
            if (Input.GetKey(KeyCode.O)) moveInput.y = 1;  // Up
            else if (Input.GetKey(KeyCode.K)) moveInput.x = -1; // Left
            else if (Input.GetKey(KeyCode.L)) moveInput.y = -1; // Down
            else if (Input.GetKey(KeyCode.Semicolon)) moveInput.x = 1; // Right
            else moveInput = Vector2.zero;

            if (moveInput == Vector2.zero) return;

            Vector3 movePlayerInputDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            Vector3 moveDirection = Vector3.zero;

            // ��������� ���� �� ���������� ������
            float cameraAngle = _cameraTrnsform.eulerAngles.y * Mathf.Deg2Rad;

            // ���������� ������������������ ��������
            float sinAngle = Mathf.Sin(cameraAngle);
            float cosAngle = Mathf.Cos(cameraAngle);
            float cotAngle = cosAngle / sinAngle; // ctg(x) = cos(x) / sin(x)
            if (sinAngle != 0)
            {
                // ������� ��� moveDirection
                moveDirection.x = (movePlayerInputDirection.z + movePlayerInputDirection.x * cotAngle) / (sinAngle + cotAngle * cosAngle);
                moveDirection.z = (moveDirection.x * cosAngle - movePlayerInputDirection.x) / sinAngle;
            }
            else
            {
                moveDirection = movePlayerInputDirection;
            }

            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
            }

            Vector3 velocity = moveDirection * _runSpeed;
            _rb.linearVelocity = new Vector3(velocity.x, _rb.linearVelocity.y, velocity.z);
        }
    }

}
