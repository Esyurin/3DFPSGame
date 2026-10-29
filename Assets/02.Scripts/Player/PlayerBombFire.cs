using System;
using Game;
using UnityEngine;
using Weapon;

namespace Player
{
    public class PlayerBombFire : MonoBehaviour
    {
        private static readonly int s_throw = Animator.StringToHash("Throw");
        [SerializeField] private Transform _firePosition;
        [SerializeField] private BombPool _bombPool;
        [SerializeField] private float _baseThrowPower = 15f;
        [SerializeField] private int _maxBombCount = 3;
        [SerializeField] private float _maxHoldTime = 2f;
        [SerializeField] private float _maxThrowForce = 30f;
        [SerializeField] private float _throwFadeTime = 0.3f; // 던진 뒤 팔이 총으로 돌아오는 시간 (s)

        private Camera _camera;
        private Animator _animator;
        private int _throwLayer;
        private PlayerWeaponMode _weaponMode;

        private int _bombCount;
        private float _holdTime;
        private float _throwForce;
        private float _throwWeight;

        public int BombCount => _bombCount;
        public event Action<int> OnBombChanged;

        private void Awake()
        {
            _camera = Camera.main;
            _bombCount = _maxBombCount;
            _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
            _throwLayer = _animator.GetLayerIndex("Throw Layer");
            _weaponMode = GetComponentInChildren<PlayerWeaponMode>();
        }

        private void Update()
        {
            UpdateThrowWeight(); // 게임 상태와 상관없이 팔은 돌려놓는다.

            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (_weaponMode.Mode != WeaponMode.Normal)
                return;

            if (_bombCount <= 0)
                return;

            if (Input.GetMouseButtonDown(1))
            {
                _holdTime = 0f;
                _throwForce = _baseThrowPower;
            }

            if (Input.GetMouseButton(1))
            {
                _holdTime += Time.deltaTime;
                float holdTimeRate = MathF.Min(1f, _holdTime / _maxHoldTime);
                _throwForce = Mathf.Lerp(_baseThrowPower, _maxThrowForce, holdTimeRate);
            }

            if (Input.GetMouseButtonUp(1))
            {
                Bomb bomb = _bombPool.GetBomb(_firePosition.position);
                if (!bomb.TryGetComponent<Rigidbody>(out Rigidbody bombRigidbody))
                    return;

                bombRigidbody.AddForce(_camera.transform.forward * _throwForce, ForceMode.Impulse);
                _bombCount--;

                _throwWeight = 1f;
                _animator.SetTrigger(s_throw);

                OnBombChanged?.Invoke(_bombCount);
            }
        }

        private void UpdateThrowWeight()
        {
            AnimatorStateInfo throwStateInfo = _animator.GetCurrentAnimatorStateInfo(_throwLayer);
            if (throwStateInfo.IsName("Throw") && throwStateInfo.normalizedTime >= 0.5f)
            {
                float maxWeight = Time.deltaTime / _throwFadeTime;
                _throwWeight = Mathf.MoveTowards(_throwWeight, 0f, maxWeight);
            }

            _animator.SetLayerWeight(_throwLayer, _throwWeight);
        }
    }
}
