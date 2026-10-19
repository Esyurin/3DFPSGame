using UnityEngine;

public class FpsCameraRotate : MonoBehaviour
{
    private const string MouseXAxis = "Mouse X";
    private const string MouseYAxis = "Mouse Y";

    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        // Take mouse input from the user
        float mouseX = Input.GetAxis(MouseXAxis);
        float mouseY = Input.GetAxis(MouseYAxis);

        // Calculate direction to rotate
        var direction = new Vector3(-mouseY, mouseX, 0);

        // Rotate the camera based on the mouse input
        transform.Rotate(_rotationSpeed * Time.deltaTime * direction);

        // Restrict vertical rotation
        Vector3 rotation = transform.eulerAngles;
        rotation.x = Mathf.Clamp(Mathf.DeltaAngle(0f, rotation.x), -90f, 90f);
        transform.eulerAngles = rotation;
    }
}
