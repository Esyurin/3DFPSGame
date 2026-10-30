using Game;
using UnityEngine;

namespace Player
{
    // 근접 모드에서 마우스 오른쪽 버튼을 누르면 오른손의 검을 휘두른다.
    public class PlayerSwordSwing : MonoBehaviour
    {
        private static readonly int s_swing = Animator.StringToHash("Swing");
        [SerializeField] private GameObject _sword; // 오른손에 붙은 검 (근접 모드에서만 보인다)
        [SerializeField] private float _swingFadeTime = 0.3f; // 휘두른 뒤 팔이 총으로 돌아오는 시간 (s)

        private Animator _animator;
        private PlayerWeaponMode _weaponMode;
        private int _swingLayer;
        private float _swingWeight;
        private bool _hasEnteredSwing;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
            _swingLayer = _animator.GetLayerIndex("Sword Layer");
            _weaponMode = GetComponentInChildren<PlayerWeaponMode>();
        }

        private void OnEnable()
        {
            // 구독: 무기 모드가 바뀌면 불러 달라고 등록한다.
            _weaponMode.OnModeChanged += RefreshSword;
        }

        private void OnDisable()
        {
            // 구독 취소
            _weaponMode.OnModeChanged -= RefreshSword;
        }

        private void Start()
        {
            RefreshSword(_weaponMode.Mode); // 처음 모드에 맞춰 검을 보이거나 숨긴다.
        }

        private void Update()
        {
            UpdateSwingWeight(); // 게임 상태와 상관없이 팔은 돌려놓는다.

            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (_weaponMode.Mode != WeaponMode.Melee)
                return;

            // 팔이 총으로 다 돌아온 뒤에만 다시 휘두른다. (휘두르는 도중 연타 막기)
            if (Input.GetMouseButtonDown(1) && _swingWeight <= 0f)
            {
                _swingWeight = 1f;
                _animator.SetTrigger(s_swing);
            }
        }

        // 무기 모드가 바뀌었다는 알림이 오면 불리는 함수(콜백). 검은 근접 모드에서만 보인다.
        private void RefreshSword(WeaponMode mode)
        {
            _sword.SetActive(mode == WeaponMode.Melee);
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
                    _hasEnteredSwing = false;
            }

            _animator.SetLayerWeight(_swingLayer, _swingWeight);
        }
    }
}
