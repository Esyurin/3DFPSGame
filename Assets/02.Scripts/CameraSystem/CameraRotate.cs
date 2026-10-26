using Game;
using UnityEngine;

namespace CameraSystem
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _minPitch = -90f;
        [SerializeField] private float _maxPitch = 90f;

        private float _yaw;
        private float _pitch;

        private void Start()
        {
            Vector3 euler = transform.eulerAngles;
            _yaw = euler.y;
            _pitch = Mathf.DeltaAngle(0f, euler.x);
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            // 커서가 풀려 있으면(UI를 누르는 중이면) 시선을 돌리지 않는다.
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            _yaw += Input.GetAxis("Mouse X") * _sensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * _sensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}
