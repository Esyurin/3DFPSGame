using UnityEngine;

namespace Player
{
    public class PlayerRotate : MonoBehaviour
    {
        private const string MouseXAxis = "Mouse X";

        [SerializeField] private float _sensitivity = 2f;

        private void Update()
        {
            float mouseX = Input.GetAxis(MouseXAxis);
            transform.Rotate(0f, mouseX * _sensitivity, 0f);
        }
    }
}