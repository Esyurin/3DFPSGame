using Core;
using Game;
using UnityEngine;

namespace Player
{
    // 근접 모드에서 마우스 오른쪽 버튼을 누르면 오른손의 검을 휘둘러, 칼날에 닿은 대상에 대미지를 준다.
    public class PlayerSwordSwing : MonoBehaviour
    {
        private static readonly int s_swing = Animator.StringToHash("Swing");
        [SerializeField] private float _damage = 25f; // 대미지
        [SerializeField] private GameObject _sword; // 오른손에 붙은 검 (근접 모드에서만 보인다)
        [SerializeField] private float _swingFadeTime = 0.3f; // 휘두른 뒤 팔이 총으로 돌아오는 시간 (s)

        private Animator _animator;
        private PlayerWeaponMode _weaponMode;
        private Collider _bladeCollider; // 검의 칼날 (트리거)
        private int _swingLayer;
        private float _swingWeight;
        private bool _hasEnteredSwing;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
            _swingLayer = _animator.GetLayerIndex("Sword Layer");
            _weaponMode = GetComponentInChildren<PlayerWeaponMode>();
            _bladeCollider = _sword.GetComponentInChildren<Collider>(true); // 자식 Blade의 콜라이더 (꺼져 있어도 찾는다)
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
            RefreshSword(_weaponMode.Mode); // 처음 모드에 맞춰 검을 보이거나 숨기고, 칼날은 꺼 둔다.
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
                DeactivateCollider(); // 지난번에 켜진 채 남은 칼날이 있으면 끄고 시작한다.
                _animator.SetTrigger(s_swing);
            }
        }

        // 칼날을 켠다. Slash 동작의 ActivateCollider 이벤트가 불러 준다.
        public void ActivateCollider()
        {
            _bladeCollider.enabled = true;
        }

        // 칼날을 끈다. Slash 동작의 DeactivateCollider 이벤트가 불러 준다.
        public void DeactivateCollider()
        {
            _bladeCollider.enabled = false;
        }

        // 칼날에 무언가가 닿으면 MeleeWeapon이 불러 준다.
        public void Hit(Collider other)
        {
            // 1. 내 몸(Player의 Character Controller)에 닿은 것은 무시한다.
            if (other.transform == transform)
                return;

            // 2. 대미지를 받을 수 없으면 무시한다.
            if (!other.TryGetComponent(out IDamageable damageable))
                return;

            // 3. 한 번 휘두를 때 하나만 맞힌다. 칼날을 바로 끈다.
            DeactivateCollider();

            // 4. 대상에서 플레이어에 가장 가까운 점을 맞은 자리로 삼는다.
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            GiveDamage(damageable, hitPoint);
        }

        // 대미지 정보를 만들어 넘긴다. 맞은 자리의 방향은 플레이어 쪽이다.
        private void GiveDamage(IDamageable damageable, Vector3 hitPoint)
        {
            Vector3 hitNormal = (transform.position - hitPoint).normalized;
            damageable.TakeDamage(new DamageInfo(_damage, hitPoint, hitNormal));
        }

        // 무기 모드가 바뀌었다는 알림이 오면 불리는 함수(콜백). 검은 근접 모드에서만 보인다.
        private void RefreshSword(WeaponMode mode)
        {
            _sword.SetActive(mode == WeaponMode.Melee);
            DeactivateCollider(); // 휘두르는 도중에 모드를 바꿔도 칼날이 켜진 채 남지 않게 한다.
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
