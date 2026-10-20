using Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private const float Gravity = -9.81f / 16f;
        private const int MaxJumpCount = 2;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _baseMoveSpeed = 6f;
        [SerializeField] private float _dashSpeed = 12f;
        [SerializeField] private float _jumpForce = 0.5f;
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _staminaRegenRate = 10f;
        [SerializeField] private float _dashStaminaRate = 20f;
        [SerializeField] private float _jumpStamina = 30f;

        private float _yVelocity; // Vertical velocity increased by gravity on each frame
        private int _jumpCount;
        private float _stamina;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _stamina = _maxStamina;
        }

        private void Update()
        {
            Move();
            RegenerateStamina();
        }

        private void Move()
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
            direction = transform.TransformDirection(direction);

            float moveSpeed = _baseMoveSpeed;
            if (Input.GetButton("Dash"))
            {
                moveSpeed = _stamina > 0f ? _dashSpeed : _baseMoveSpeed;
                _stamina = Mathf.Max(0f, _stamina - _dashStaminaRate * Time.deltaTime);
            }

            if (_characterController.isGrounded)
            {
                _jumpCount = 0;
                _yVelocity = 0f;
            }
            else
            {
                _yVelocity += Gravity * Time.deltaTime;
            }

            if (Input.GetButtonDown("Jump") && _jumpCount < MaxJumpCount && _stamina > _jumpStamina)
            {
                _yVelocity = _jumpForce;
                _jumpCount++;
                _stamina -= _jumpStamina;
            }

            Vector3 velocity = direction * (moveSpeed * Time.deltaTime) + Vector3.up * _yVelocity;
            _characterController.Move(velocity);
        }

        private void RegenerateStamina()
        {
            _stamina = Mathf.Min(_maxStamina, _stamina + _staminaRegenRate * Time.deltaTime);
        }
    }
}