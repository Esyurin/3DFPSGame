using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private const float Gravity = -9.81f / 8f;

        [SerializeField] private float _baseMoveSpeed = 6f;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _jumpForce = 0.5f;

        private float _yVelocity; // Vertical velocity increased by gravity on each frame

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();
            // 2단 점프: 공중에서 한 더 뛰는 2단 점프를 만듭니다. 
            //     스태미나를 만들어서 적용합니다. (스태미나가 있을 경우에만 질주/점프 가능)
            // 질주시 스태미나를 소모합니다.
            //     점프시 스태미나를 소모합니다.
        }

        private void Move()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
            direction = transform.TransformDirection(direction);

            float moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 12f : _baseMoveSpeed;

            if (_characterController.isGrounded)
                _yVelocity = Input.GetButtonDown("Jump") ? _jumpForce : -1f;
            else
                _yVelocity += Gravity * Time.deltaTime;

            Vector3 velocity = direction * (moveSpeed * Time.deltaTime) + Vector3.up * _yVelocity;
            _characterController.Move(velocity);
        }
    }
}