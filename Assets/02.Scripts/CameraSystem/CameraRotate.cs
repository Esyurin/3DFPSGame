using Game;
using Player;
using UnityEngine;

namespace CameraSystem
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private float _sensitivity = 2f;
        [SerializeField] private float _minPitch = -90f;
        [SerializeField] private float _maxPitch = 90f;
        [SerializeField] private PlayerZoom _playerZoom; // 확대 중이면 감도를 낮춘다.

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

            float sensitivity = _sensitivity * _playerZoom.SensitivityRate;
            _yaw += Input.GetAxis("Mouse X") * sensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * sensitivity;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);

            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
    }
}
