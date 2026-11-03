using System.Collections;
using UnityEngine;

namespace UI
{
    // 잘못 입력했을 때 패널을 좌우로 짧게 흔든다.
    public class UIShake : MonoBehaviour
    {
        [SerializeField] private float _duration = 0.35f;
        [SerializeField] private float _strength = 14f;
        [SerializeField] private float _frequency = 40f;

        private RectTransform _rect;
        private Coroutine _shakeRoutine;
        private Vector2 _origin;

        private void Awake()
        {
            _rect = (RectTransform)transform;
        }

        public void Play()
        {
            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
                _rect.anchoredPosition = _origin;
            }

            _shakeRoutine = StartCoroutine(ShakeRoutine());
        }

        private IEnumerator ShakeRoutine()
        {
            _origin = _rect.anchoredPosition;

            for (float time = 0f; time < _duration; time += Time.unscaledDeltaTime)
            {
                float damping = 1f - time / _duration;
                float x = Mathf.Sin(time * _frequency) * _strength * damping;
                _rect.anchoredPosition = _origin + new Vector2(x, 0f);
                yield return null;
            }

            _rect.anchoredPosition = _origin;
            _shakeRoutine = null;
        }
    }
}
