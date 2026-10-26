using Game;
using UnityEngine;

namespace Player
{
    public class PlayerRotate : MonoBehaviour
    {
        private const string MouseXAxis = "Mouse X";

        [SerializeField] private float _sensitivity = 2f;

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            // 커서가 풀려 있으면(UI를 누르는 중이면) 시선을 돌리지 않는다.
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            float mouseX = Input.GetAxis(MouseXAxis);
            transform.Rotate(0f, mouseX * _sensitivity, 0f);
        }
    }
}
