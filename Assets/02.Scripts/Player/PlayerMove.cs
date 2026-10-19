using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    // Move player depending on keyboard
    private void Update()
    {
        // Get keyboard input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate direction to move
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
        direction = transform.TransformDirection(direction);

        // Move player
        transform.position += direction * (_moveSpeed * Time.deltaTime);
    }
}