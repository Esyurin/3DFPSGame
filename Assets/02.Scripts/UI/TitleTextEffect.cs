using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // 로그인 화면 제목 연출.
    // 등장: 붉은 베기 선이 지나가면, 선이 지나간 글자부터 무작위 문자로 돌다가 원래 글자로 맞춰진다.
    // 대기: 가끔 글자가 옆으로 튀며 빨강·청록 잔상이 생기고(글리치), 한 글자가 고장 난 네온처럼 깜빡인다.
    public class TitleTextEffect : MonoBehaviour
    {
        private const string ScrambleChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#%&*";

        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private TextMeshProUGUI _subtitle;
        [SerializeField] private Image _slash;

        [Header("등장")]
        [SerializeField] private float _slashDuration = 0.35f;
        [SerializeField] private float _slashAngle = 4f;
        [SerializeField] private float _scrambleDuration = 0.45f;
        [SerializeField] private float _scrambleInterval = 0.04f;

        [Header("글리치")]
        [SerializeField] private Vector2 _glitchDelay = new Vector2(2.5f, 5f);
        [SerializeField] private float _glitchDuration = 0.18f;
        [SerializeField] private float _glitchOffset = 10f;
        [SerializeField] private float _ghostOffset = 6f;
        [SerializeField] private Color _ghostRedColor = new Color(1f, 0.15f, 0.2f, 0.7f);
        [SerializeField] private Color _ghostCyanColor = new Color(0.2f, 0.9f, 1f, 0.7f);

        [Header("깜빡임")]
        [SerializeField] private int _flickerIndex = 6; // "DEAD ZONE"의 O
        [SerializeField] private Vector2 _flickerDelay = new Vector2(3f, 7f);

        private readonly StringBuilder _builder = new StringBuilder();
        private string _finalTitle;
        private float[] _charPositions;
        private TextMeshProUGUI _ghostRed;
        private TextMeshProUGUI _ghostCyan;
        private float _subtitleSpacing;

        private bool _isIdle;
        private bool _needsReset;
        private float _flickerAlpha = 1f;
        private int _glitchIndexA = -1;
        private int _glitchIndexB = -1;
        private float _glitchShift;

        private void Awake()
        {
            _finalTitle = _title.text;
            _subtitleSpacing = _subtitle.characterSpacing;

            _ghostRed = CreateGhost("TitleGhostRed");
            _ghostCyan = CreateGhost("TitleGhostCyan");

            CacheCharPositions();

            _title.text = Hidden(_finalTitle);
            _subtitle.alpha = 0f;
            _slash.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        }

        private void LateUpdate()
        {
            if (!_isIdle)
                return;

            bool isActive = _flickerAlpha < 1f || _glitchIndexA >= 0;
            if (!isActive && !_needsReset)
                return;

            // 매번 원래 메시로 되돌린 뒤 이번 프레임의 효과만 다시 입힌다.
            _title.ForceMeshUpdate();
            _needsReset = isActive;
            if (!isActive)
                return;

            TMP_TextInfo textInfo = _title.textInfo;
            ApplyAlpha(textInfo, _flickerIndex, _flickerAlpha);
            ApplyShift(textInfo, _glitchIndexA, _glitchShift);
            ApplyShift(textInfo, _glitchIndexB, -_glitchShift * 0.6f);
            _title.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32 | TMP_VertexDataUpdateFlags.Vertices);
        }

        public IEnumerator PlayIntroRoutine()
        {
            PlaceSlash();
            float[] nextScrambleTimes = new float[_finalTitle.Length];
            char[] shown = _finalTitle.ToCharArray();
            float totalDuration = _slashDuration + _scrambleDuration;

            for (float time = 0f; time < totalDuration; time += Time.unscaledDeltaTime)
            {
                float slash = Mathf.Clamp01(time / _slashDuration);
                _slash.rectTransform.localScale = new Vector3(UIEase.OutCubic(slash), 1f, 1f);

                _builder.Clear();
                for (int i = 0; i < _finalTitle.Length; i++)
                {
                    char final = _finalTitle[i];
                    float revealTime = _charPositions[i] * _slashDuration;

                    if (final == ' ' || time >= revealTime + _scrambleDuration)
                    {
                        _builder.Append(final);
                    }
                    else if (time >= revealTime)
                    {
                        if (time >= nextScrambleTimes[i])
                        {
                            shown[i] = ScrambleChars[Random.Range(0, ScrambleChars.Length)];
                            nextScrambleTimes[i] = time + _scrambleInterval;
                        }

                        _builder.Append(shown[i]);
                    }
                    else
                    {
                        _builder.Append("<alpha=#00>").Append(final).Append("<alpha=#FF>");
                    }
                }

                _title.text = _builder.ToString();
                yield return null;
            }

            _title.text = _finalTitle;

            // 베기 선이 사라지는 동안 부제가 넓은 간격에서 제자리로 모이며 나타난다.
            yield return Tween(0.5f, t =>
            {
                float e = UIEase.OutCubic(t);
                _slash.color = new Color(_slash.color.r, _slash.color.g, _slash.color.b, 1f - e);
                _subtitle.alpha = e;
                _subtitle.characterSpacing = Mathf.Lerp(_subtitleSpacing * 2f, _subtitleSpacing, e);
            });

            _isIdle = true;
            StartCoroutine(GlitchLoopRoutine());
            StartCoroutine(FlickerLoopRoutine());
        }

        private IEnumerator GlitchLoopRoutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Random.Range(_glitchDelay.x, _glitchDelay.y));

                _glitchIndexA = RandomLetterIndex();
                _glitchIndexB = Random.value < 0.5f ? RandomLetterIndex() : -1;
                _ghostRed.alpha = _ghostRedColor.a;
                _ghostCyan.alpha = _ghostCyanColor.a;

                for (float time = 0f; time < _glitchDuration; time += 0.04f)
                {
                    _glitchShift = Random.Range(-_glitchOffset, _glitchOffset);
                    float ghost = Random.Range(0.5f, 1f) * _ghostOffset;
                    _ghostRed.rectTransform.anchoredPosition = _title.rectTransform.anchoredPosition + new Vector2(-ghost, 0f);
                    _ghostCyan.rectTransform.anchoredPosition = _title.rectTransform.anchoredPosition + new Vector2(ghost, 0f);
                    yield return new WaitForSecondsRealtime(0.04f);
                }

                _glitchIndexA = -1;
                _glitchIndexB = -1;
                _ghostRed.alpha = 0f;
                _ghostCyan.alpha = 0f;
            }
        }

        private IEnumerator FlickerLoopRoutine()
        {
            float[] blinks = { 0.15f, 1f, 0.05f, 0.6f, 0.1f, 1f };

            while (true)
            {
                yield return new WaitForSecondsRealtime(Random.Range(_flickerDelay.x, _flickerDelay.y));

                foreach (float alpha in blinks)
                {
                    _flickerAlpha = alpha;
                    yield return new WaitForSecondsRealtime(Random.Range(0.03f, 0.09f));
                }

                _flickerAlpha = 1f;
            }
        }

        private TextMeshProUGUI CreateGhost(string ghostName)
        {
            GameObject ghost = Instantiate(_title.gameObject, _title.transform.parent);
            ghost.name = ghostName;
            ghost.transform.SetSiblingIndex(_title.transform.GetSiblingIndex());

            var text = ghost.GetComponent<TextMeshProUGUI>();
            text.color = ghostName == "TitleGhostRed" ? _ghostRedColor : _ghostCyanColor;
            text.alpha = 0f;
            text.raycastTarget = false;
            return text;
        }

        // 글자마다 제목 왼쪽 끝에서 얼마나 떨어져 있는지(0~1)를 구해 둔다. 베기 선이 그 위치를 지날 때 글자가 나타난다.
        private void CacheCharPositions()
        {
            _title.ForceMeshUpdate();
            TMP_TextInfo textInfo = _title.textInfo;
            Bounds bounds = _title.textBounds;

            _charPositions = new float[_finalTitle.Length];
            for (int i = 0; i < _finalTitle.Length; i++)
            {
                TMP_CharacterInfo info = textInfo.characterInfo[i];
                float center = (info.bottomLeft.x + info.topRight.x) * 0.5f;
                _charPositions[i] = Mathf.InverseLerp(bounds.min.x, bounds.max.x, center);
            }
        }

        // 베기 선을 제목 글자 영역 왼쪽 아래에서 오른쪽 위로 걸치게 놓는다. 선의 가운데가 글자 가운데를 지난다.
        private void PlaceSlash()
        {
            Bounds bounds = _title.textBounds;
            RectTransform slash = _slash.rectTransform;
            Transform parent = slash.parent;

            float angle = _slashAngle * Mathf.Deg2Rad;
            float width = (bounds.size.x + 60f) / Mathf.Cos(angle);
            float rise = width * Mathf.Sin(angle);

            Vector3 start = _title.rectTransform.TransformPoint(new Vector3(bounds.min.x - 30f, bounds.center.y - rise * 0.5f, 0f));
            slash.position = start;
            slash.localEulerAngles = new Vector3(0f, 0f, _slashAngle);
            slash.sizeDelta = new Vector2(width / parent.lossyScale.x * _title.rectTransform.lossyScale.x, slash.sizeDelta.y);
            _slash.color = new Color(_slash.color.r, _slash.color.g, _slash.color.b, 1f);
        }

        private int RandomLetterIndex()
        {
            int index;
            do
            {
                index = Random.Range(0, _finalTitle.Length);
            }
            while (_finalTitle[index] == ' ');

            return index;
        }

        private static string Hidden(string text)
        {
            return "<alpha=#00>" + text;
        }

        private static void ApplyAlpha(TMP_TextInfo textInfo, int index, float alpha)
        {
            if (alpha >= 1f || !IsVisible(textInfo, index))
                return;

            TMP_CharacterInfo info = textInfo.characterInfo[index];
            Color32[] colors = textInfo.meshInfo[info.materialReferenceIndex].colors32;
            for (int v = 0; v < 4; v++)
                colors[info.vertexIndex + v].a = (byte)(colors[info.vertexIndex + v].a * alpha);
        }

        private static void ApplyShift(TMP_TextInfo textInfo, int index, float shift)
        {
            if (!IsVisible(textInfo, index))
                return;

            TMP_CharacterInfo info = textInfo.characterInfo[index];
            Vector3[] vertices = textInfo.meshInfo[info.materialReferenceIndex].vertices;
            for (int v = 0; v < 4; v++)
                vertices[info.vertexIndex + v] += new Vector3(shift, 0f, 0f);
        }

        private static bool IsVisible(TMP_TextInfo textInfo, int index)
        {
            return index >= 0 && index < textInfo.characterCount && textInfo.characterInfo[index].isVisible;
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
