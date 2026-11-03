using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    // 마우스를 올리면 버튼이 살짝 커지고, 누르면 살짝 줄어든다.
    public class UIButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float _hoverScale = 1.05f;
        [SerializeField] private float _pressScale = 0.96f;
        [SerializeField] private float _speed = 14f;

        private float _targetScale = 1f;
        private bool _isHovered;

        private void OnDisable()
        {
            _isHovered = false;
            _targetScale = 1f;
            transform.localScale = Vector3.one;
        }

        private void Update()
        {
            float scale = Mathf.Lerp(transform.localScale.x, _targetScale, 1f - Mathf.Exp(-_speed * Time.unscaledDeltaTime));
            transform.localScale = Vector3.one * scale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovered = true;
            _targetScale = _hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovered = false;
            _targetScale = 1f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _targetScale = _pressScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _targetScale = _isHovered ? _hoverScale : 1f;
        }
    }
}
