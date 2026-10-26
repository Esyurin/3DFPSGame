using System;
using System.Collections;
using Core;
using UnityEngine;

namespace Monster
{
    [RequireComponent(typeof(CharacterController))]
    public class Zombie : MonoBehaviour, IDamageable
    {
        [Header("Effect")]
        [SerializeField] private ParticleSystem _bloodSprayPrefab;

        [Header("State")]
        [SerializeField] private MonsterState _state = MonsterState.Idle;

        [Header("Stats")]
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _moveSpeed = 4f;

        [Header("Distance (m)")]
        [SerializeField] private float _detectDistance = 8f;
        [SerializeField] private float _attackDistance = 1.5f;
        [SerializeField] private float _comebackDistance = 15f;
        [SerializeField] private float _arriveDistance = 0.5f;

        [Header("Time (s)")]
        [SerializeField] private float _attackInterval = 2f;
        [SerializeField] private float _hitTime = 0.3f;
        [SerializeField] private float _deathTime = 2f;

        private CharacterController _controller;
        private Player.Player _player;
        private float _attackTimer;
        private Vector3 _startPosition;
        private Coroutine _hitCoroutine;

        public float Health { get; private set; }

        public float MaxHealth => _maxHealth;

        public event Action OnDamaged;
        public event Action OnDeath;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null || !playerObject.TryGetComponent(out _player))
            {
                Debug.LogError($"{name}: Player를 찾을 수 없습니다.");
                enabled = false;
                return;
            }

            Health = _maxHealth;
            _startPosition = transform.position;
        }

        private void Update()
        {
            if (_attackTimer > 0f)
                _attackTimer -= Time.deltaTime;

            switch (_state)
            {
                case MonsterState.Idle:
                    Idle();
                    break;
                case MonsterState.Trace:
                    Trace();
                    break;
                case MonsterState.Comeback:
                    Comeback();
                    break;
                case MonsterState.Attack:
                    Attack();
                    break;
            }
        }

        private void ChangeState(MonsterState nextState)
        {
            Debug.Log($"{name}: {_state} -> {nextState}");
            _state = nextState;
        }

        private void Idle()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= _detectDistance)
                ChangeState(MonsterState.Trace);
        }

        private void Trace()
        {
            if (Vector3.Distance(transform.position, _startPosition) > _comebackDistance)
            {
                ChangeState(MonsterState.Comeback);
                return;
            }

            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance <= _attackDistance)
            {
                ChangeState(MonsterState.Attack);
                return;
            }

            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();
            transform.forward = direction;
            _controller.SimpleMove(direction * _moveSpeed);
        }

        private void Comeback()
        {
            if (Vector3.Distance(transform.position, _startPosition) < _arriveDistance)
            {
                Health = _maxHealth;
                ChangeState(MonsterState.Idle);
                return;
            }

            Vector3 direction = _startPosition - transform.position;
            direction.y = 0f;
            direction.Normalize();
            transform.forward = direction;
            _controller.SimpleMove(direction * _moveSpeed);
        }

        private void Attack()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            if (distance > _attackDistance)
            {
                ChangeState(MonsterState.Trace);
                return;
            }

            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
                transform.forward = direction.normalized;

            if (_attackTimer <= 0f)
            {
                _attackTimer = _attackInterval;
                _player.TakeDamage(new DamageInfo(_damage));
            }
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_state == MonsterState.Death)
                return;

            Health = Mathf.Max(0f, Health - damageInfo.Value);
            OnDamaged?.Invoke();

            SpawnBloodSpray(damageInfo);

            if (Health > 0f)
            {
                if (_hitCoroutine != null)
                    StopCoroutine(_hitCoroutine);
                ChangeState(MonsterState.Hit);
                _hitCoroutine = StartCoroutine(HitRoutine());
            }
            else
            {
                StopAllCoroutines();
                ChangeState(MonsterState.Death);
                OnDeath?.Invoke();
                StartCoroutine(DeathRoutine());
            }
        }

        private void SpawnBloodSpray(DamageInfo damageInfo)
        {
            if (_bloodSprayPrefab == null)
                return;

            Quaternion rotation = damageInfo.HitNormal == Vector3.zero
                ? Quaternion.identity
                : Quaternion.LookRotation(damageInfo.HitNormal);
            Instantiate(_bloodSprayPrefab, damageInfo.HitPoint, rotation, transform);
        }

        private IEnumerator HitRoutine()
        {
            yield return new WaitForSeconds(_hitTime);
            _hitCoroutine = null;
            ChangeState(MonsterState.Trace);
        }

        private IEnumerator DeathRoutine()
        {
            _controller.enabled = false;
            yield return new WaitForSeconds(_deathTime);
            Destroy(gameObject);
        }
    }
}
