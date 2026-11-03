using UnityEngine;

namespace CameraSystem
{
    // 로그인 화면(A안)에서 카메라를 처음 각도 기준으로 천천히 좌우·위아래로 흔든다.
    public class LoginCameraSway : MonoBehaviour
    {
        [SerializeField] private float _yawAmount = 3f;
        [SerializeField] private float _pitchAmount = 1f;
        [SerializeField] private float _speed = 0.15f;

        private Quaternion _baseRotation;

        private void Start()
        {
            _baseRotation = transform.rotation;
        }

        private void Update()
        {
            float t = Time.time * _speed * Mathf.PI * 2f;
            float yaw = Mathf.Sin(t) * _yawAmount;
            float pitch = Mathf.Sin(t * 0.5f) * _pitchAmount;

            transform.rotation = _baseRotation * Quaternion.Euler(pitch, yaw, 0f);
        }
    }
}
