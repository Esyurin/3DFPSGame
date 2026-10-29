using Game;
using UnityEngine;

namespace Player
{
    public class PlayerSwordSwing : MonoBehaviour
    {
        private static readonly int s_swing = Animator.StringToHash("Swing");
        [SerializeField] private GameObject _sword; // 오른손에 붙은 검 (휘두를 때만 보인다)
        [SerializeField] private KeyCode _swingKey = KeyCode.F;
        [SerializeField] private float _swingFadeTime = 0.3f; // 휘두른 뒤 팔이 총으로 돌아오는 시간 (s)

        private Animator _animator;
        private int _swingLayer;
        private float _swingWeight;
        private bool _hasEnteredSwing;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
            _swingLayer = _animator.GetLayerIndex("Sword Layer");
            _sword.SetActive(false);
        }

        private void Update()
        {
            UpdateSwingWeight(); // 게임 상태와 상관없이 팔은 돌려놓는다.

            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (Input.GetKeyDown(_swingKey) && _swingWeight <= 0f)
            {
                _swingWeight = 1f;
                _sword.SetActive(true);
                _animator.SetTrigger(s_swing);
            }
        }

        private void UpdateSwingWeight()
        {
            AnimatorStateInfo swingStateInfo = _animator.GetCurrentAnimatorStateInfo(_swingLayer);
            if (swingStateInfo.IsName("Swing"))
                _hasEnteredSwing = true;

            // 휘두르기 끝부분이거나, 이미 Empty로 돌아왔으면 팔을 되돌린다.
            bool isSwingEnding = swingStateInfo.IsName("Swing") && swingStateInfo.normalizedTime >= 0.8f;
            bool isBackToEmpty = _hasEnteredSwing && !swingStateInfo.IsName("Swing");
            if (isSwingEnding || isBackToEmpty)
            {
                float maxWeight = Time.deltaTime / _swingFadeTime;
                _swingWeight = Mathf.MoveTowards(_swingWeight, 0f, maxWeight);
                if (_swingWeight <= 0f)
                {
                    _sword.SetActive(false);
                    _hasEnteredSwing = false;
                }
            }

            _animator.SetLayerWeight(_swingLayer, _swingWeight);
        }
    }
}
