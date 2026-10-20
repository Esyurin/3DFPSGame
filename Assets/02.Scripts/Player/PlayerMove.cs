using Game;
using UnityEngine;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        private const float Gravity = -9.81f;

        [SerializeField] private float _moveSpeed;
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _jumpForce = 0.5f;

        private float _yVelocity; // Vertical velocity increased by gravity on each frame

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        // Move player depending on keyboard
        private void Update()
        {
            // Get keyboard input
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            // GameLogger.Log($"Horizontal input: {horizontalInput}, Vertical input: {verticalInput}");

            // Calculate direction to move
            Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
            direction = transform.TransformDirection(direction);

            if (!_characterController.isGrounded)
                _yVelocity += Gravity * Time.deltaTime;
            else
                _yVelocity = 0;

            if (Input.GetButtonDown("Jump") && _characterController.isGrounded)
                _yVelocity = _jumpForce;

            Vector3 velocity = direction * (_moveSpeed * Time.deltaTime) + Vector3.up * _yVelocity;

            // Move player
            _characterController.Move(velocity);
        }
    }
}