using Game;
using UnityEngine;

namespace Player
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed;
        [SerializeField] private CharacterController _characterController;

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

            // Move player
            _characterController.SimpleMove(direction * _moveSpeed);
        }
    }
}