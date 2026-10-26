using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class HitEffectUI : MonoBehaviour
    {
        [SerializeField] private Player.Player _player;
        [SerializeField] private float _maxAlpha = 0.4f;
        [SerializeField] private float _fadeTime = 0.3f;

        private Image _image;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _image = GetComponent<Image>();
            SetAlpha(0f);
        }

        private void OnEnable()
        {
            _player.OnDamaged += Play;
        }

        private void OnDisable()
        {
            _player.OnDamaged -= Play;
        }

        public void Play()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeRoutine());
        }

        private IEnumerator FadeRoutine()
        {
            float elapsed = 0f;
            while (elapsed < _fadeTime)
            {
                SetAlpha(Mathf.Lerp(_maxAlpha, 0f, elapsed / _fadeTime));
                elapsed += Time.deltaTime;
                yield return null;
            }

            SetAlpha(0f);
            _fadeCoroutine = null;
        }

        private void SetAlpha(float alpha)
        {
            Color color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
