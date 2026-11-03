using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // 입력칸을 선택하면 테두리를 강조색으로 바꾸고, 아래쪽 밑줄이 가운데서 양옆으로 펼쳐진다.
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFocusHighlight : MonoBehaviour
    {
        [SerializeField] private Outline _outline;
        [SerializeField] private RectTransform _underline;
        [SerializeField] private Color _normalColor = new Color(1f, 1f, 1f, 0.22f);
        [SerializeField] private Color _focusColor = new Color(0.753f, 0.224f, 0.169f, 1f);
        [SerializeField] private float _speed = 12f;

        private TMP_InputField _inputField;
        private float _focus;
        private float _target;

        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onSelect.AddListener(_ => _target = 1f);
            _inputField.onDeselect.AddListener(_ => _target = 0f);
            Apply();
        }

        private void Update()
        {
            if (Mathf.Approximately(_focus, _target))
                return;

            _focus = Mathf.MoveTowards(_focus, _target, _speed * Time.unscaledDeltaTime);
            Apply();
        }

        private void Apply()
        {
            _outline.effectColor = Color.Lerp(_normalColor, _focusColor, _focus);
            _underline.localScale = new Vector3(UIEase.OutCubic(_focus), 1f, 1f);
        }
    }
}
