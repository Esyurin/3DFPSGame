using System.Collections;
using UnityEngine;

namespace UI
{
    // 로그인 화면이 열릴 때와 닫힐 때의 연출.
    // 열릴 때: 검은 화면이 걷히고 → 위아래 시네마 바가 빠지고 → 로고 → 패널 → 패널 안 항목이 차례로 나타난다.
    // 닫힐 때: 화면이 다시 검게 덮인다.
    public class LoginIntroSequence : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fader;
        [SerializeField] private RectTransform _topBar;
        [SerializeField] private RectTransform _bottomBar;
        [SerializeField] private CanvasGroup _logo;
        [SerializeField] private TitleTextEffect _titleEffect; // 있으면 로고 페이드 대신 제목 글자 연출을 쓴다.
        [SerializeField] private CanvasGroup _panel;
        [SerializeField] private RectTransform _accentBar;
        [SerializeField] private CanvasGroup[] _panelItems;
        [SerializeField] private CanvasGroup _footer;

        [SerializeField] private float _fadeInDuration = 1.2f;
        [SerializeField] private float _barDuration = 0.8f;
        [SerializeField] private float _logoDuration = 0.9f;
        [SerializeField] private float _panelDuration = 0.6f;
        [SerializeField] private float _itemInterval = 0.07f;
        [SerializeField] private float _panelSlideDistance = 220f;

        private RectTransform _logoRect;
        private RectTransform _panelRect;
        private Vector2 _panelPosition;
        private float _barHeight;

        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            _logoRect = (RectTransform)_logo.transform;
            _panelRect = (RectTransform)_panel.transform;
            _panelPosition = _panelRect.anchoredPosition;
            _barHeight = _topBar.sizeDelta.y;

            // 첫 프레임부터 연출 시작 상태로 맞춰 둔다.
            _fader.alpha = 1f;
            _fader.blocksRaycasts = true;
            _logo.alpha = 0f;
            _panel.alpha = 0f;
            _panel.interactable = false;
            _footer.alpha = 0f;
            _accentBar.localScale = new Vector3(1f, 0f, 1f);
            foreach (CanvasGroup item in _panelItems)
                item.alpha = 0f;
        }

        private void Start()
        {
            StartCoroutine(PlayIntroRoutine());
        }

        public IEnumerator PlayOutroRoutine(float duration = 0.6f)
        {
            _fader.blocksRaycasts = true;
            yield return Tween(duration, t => _fader.alpha = UIEase.InOutCubic(t));
        }

        private IEnumerator PlayIntroRoutine()
        {
            IsPlaying = true;

            yield return Tween(_fadeInDuration, t => _fader.alpha = 1f - UIEase.InOutCubic(t));
            _fader.blocksRaycasts = false;

            // 시네마 바가 화면 밖으로 빠지는 동안 로고를 띄운다.
            StartCoroutine(Tween(_barDuration, t =>
            {
                float y = _barHeight * UIEase.InOutCubic(t);
                _topBar.anchoredPosition = new Vector2(0f, y);
                _bottomBar.anchoredPosition = new Vector2(0f, -y);
            }));

            if (_titleEffect != null)
            {
                _logo.alpha = 1f;
                yield return _titleEffect.PlayIntroRoutine();
            }
            else
            {
                yield return Tween(_logoDuration, t =>
                {
                    _logo.alpha = UIEase.OutCubic(t);
                    _logoRect.localScale = Vector3.one * Mathf.LerpUnclamped(1.15f, 1f, UIEase.OutCubic(t));
                });
            }

            yield return Tween(_panelDuration, t =>
            {
                float e = UIEase.OutCubic(t);
                _panel.alpha = e;
                _panelRect.anchoredPosition = _panelPosition + new Vector2(_panelSlideDistance * (1f - e), 0f);
            });

            // 패널 왼쪽 강조선이 가운데서 위아래로 뻗는다.
            StartCoroutine(Tween(0.4f, t => _accentBar.localScale = new Vector3(1f, UIEase.OutBack(t), 1f)));

            foreach (CanvasGroup item in _panelItems)
            {
                StartCoroutine(FadeUpRoutine(item));
                yield return new WaitForSecondsRealtime(_itemInterval);
            }

            _panel.interactable = true;
            yield return Tween(0.5f, t => _footer.alpha = t);

            IsPlaying = false;
        }

        private IEnumerator FadeUpRoutine(CanvasGroup item)
        {
            var rect = (RectTransform)item.transform;
            Vector2 target = rect.anchoredPosition;

            yield return Tween(0.35f, t =>
            {
                float e = UIEase.OutCubic(t);
                item.alpha = e;
                rect.anchoredPosition = target + new Vector2(0f, -16f * (1f - e));
            });
        }

        private static IEnumerator Tween(float duration, System.Action<float> apply)
        {
            for (float time = 0f; time < duration; time += Time.unscaledDeltaTime)
            {
                apply(time / duration);
                yield return null;
            }

            apply(1f);
        }
    }
}
