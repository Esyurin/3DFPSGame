using System;
using System.Collections.Generic;
using Core;
using Game;
using Monster.States;
using UnityEngine;
using UnityEngine.AI;

namespace Monster
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Zombie : MonoBehaviour, IDamageable
    {
        private static readonly int s_stateHash = Animator.StringToHash("State");
        private static readonly int s_attackHash = Animator.StringToHash("Attack");
        private static readonly int s_hitHash = Animator.StringToHash("Hit");

        [Header("Effect")]
        [SerializeField] private ParticleSystem _bloodSprayPrefab;

        [Header("Animation")]
        [SerializeField] private Animator _animator;

        [Header("State")]
        [SerializeField] private MonsterState _state = MonsterState.Idle;

        [Header("Stats")]
        [SerializeField] private float _maxHealth = 50f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField, Range(0f, 1f)] private float _mudSpeedMultiplier = 0.5f; // 진흙 위에서 걷는 빠르기 배율

        [Header("Distance (m)")]
        [SerializeField] private float _detectDistance = 8f;
        [SerializeField] private float _attackDistance = 1.5f;
        [SerializeField] private float _comebackDistance = 15f;
        [SerializeField] private float _arriveDistance = 0.5f;
        [SerializeField] private float _roamRadius = 6f;

        [Header("Time (s)")]
        [SerializeField] private float _attackInterval = 2f;
        [SerializeField] private float _hitTime = 0.3f;
        [SerializeField] private float _deathTime = 2f;
        [SerializeField] private float _idleTime = 2f;
        [SerializeField] private float _roamTimeout = 5f;

        [Header("Patrol")]
        [SerializeField] private Transform[] _patrolWaypoints;

        private readonly Dictionary<MonsterState, ZombieState> _states = new Dictionary<MonsterState, ZombieState>();

        private ZombiePool _pool;
        private Player.Player _player;
        private ZombieState _currentState;
        private float _attackTimer;
        private Vector3 _startPosition;
        private NavMeshAgent _agent;
        private Collider _collider;
        private int _mudAreaMask;

        public float Health { get; private set; }
        public float MaxHealth => _maxHealth;

        public Player.Player Player => _player;
        public Vector3 StartPosition => _startPosition;
        public Transform[] Waypoints => _patrolWaypoints;
        public bool HasWaypoints => _patrolWaypoints != null && _patrolWaypoints.Length > 0;

        public float AttackDistance => _attackDistance;
        public float ComebackDistance => _comebackDistance;
        public float RoamRadius => _roamRadius;
        public float HitTime => _hitTime;
        public float DeathTime => _deathTime;
        public float IdleTime => _idleTime;
        public float RoamTimeout => _roamTimeout;

        public event Action OnHealthChanged;
        public event Action<Zombie> OnDeath;

        private void Awake()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null || !playerObject.TryGetComponent(out _player))
            {
                Debug.LogError($"{name}: Player를 찾을 수 없습니다.");
                enabled = false;
                return;
            }

            Health = _maxHealth;
            _startPosition = transform.position;

            // 첫 상태의 Enter에서 에이전트를 쓰므로 상태보다 먼저 가져온다.
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _moveSpeed;
            _collider = GetComponent<Collider>();

            // Areas 탭에 Mud가 없으면 -1이 돌아오므로, 그때는 진흙으로 보는 곳이 없게 0으로 둔다.
            int mudArea = NavMesh.GetAreaFromName("Mud");
            _mudAreaMask = mudArea >= 0 ? 1 << mudArea : 0;

            _states.Add(MonsterState.Idle, new ZombieIdleState(this));
            _states.Add(MonsterState.Patrol, new ZombiePatrolState(this));
            _states.Add(MonsterState.Roaming, new ZombieRoamingState(this));
            _states.Add(MonsterState.Trace, new ZombieTraceState(this));
            _states.Add(MonsterState.Attack, new ZombieAttackState(this));
            _states.Add(MonsterState.Comeback, new ZombieComebackState(this));
            _states.Add(MonsterState.Hit, new ZombieHitState(this));
            _states.Add(MonsterState.Death, new ZombieDeathState(this));

            _currentState = _states[_state];
            _currentState.Enter();
        }

        // 풀에서 꺼낼 때(Spawn)는 꺼진 상태라 애니메이터에 값을 못 넣으므로, 켜질 때 현재 상태를 맞춘다.
        private void OnEnable()
        {
            SyncAnimatorState();
        }

        public void Init(ZombiePool pool)
        {
            _pool = pool;
        }

        // 풀에서 꺼낼 때 호출한다. 꺼져 있는 동안 위치와 상태를 새로 맞춘다.
        public void Spawn(Vector3 position, Transform[] waypoints)
        {
            // NavMeshAgent가 켜져 있으면 위치를 옮겨도 덮어쓰므로 잠깐 끈다. 죽을 때 꺼둔 것도 여기서 다시 켠다.
            _agent.enabled = false;
            transform.SetPositionAndRotation(position, Quaternion.identity);
            SetBodyEnabled(true);

            _startPosition = position;
            _patrolWaypoints = waypoints;
            _attackTimer = 0f;
            Health = _maxHealth;
            OnHealthChanged?.Invoke();
            ChangeState(MonsterState.Idle);
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (_attackTimer > 0f)
                _attackTimer -= Time.deltaTime;

            UpdateMoveSpeed();
            _currentState.Update();
        }

        // Cost는 길을 고를 때만 쓰이고 빠르기는 바꾸지 않으므로, 진흙 위에 서 있으면 직접 느리게 걷게 한다.
        private void UpdateMoveSpeed()
        {
            // 죽어서 에이전트가 꺼졌거나 NavMesh 밖이면 지금 서 있는 곳을 알 수 없다.
            if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
                return;

            _agent.SamplePathPosition(NavMesh.AllAreas, 0f, out NavMeshHit hit);
            bool isOnMud = (hit.mask & _mudAreaMask) != 0;
            _agent.speed = isOnMud ? _moveSpeed * _mudSpeedMultiplier : _moveSpeed;
        }

        public void ChangeState(MonsterState nextState)
        {
            GameLogger.Log($"{name}: {_state} -> {nextState}");
            _currentState.Exit();
            _state = nextState;
            _currentState = _states[nextState];
            _currentState.Enter();
            SyncAnimatorState();
        }

        private void SyncAnimatorState()
        {
            if (_animator == null || !_animator.isActiveAndEnabled)
                return;

            _animator.SetInteger(s_stateHash, (int)_state);
            // 이전 상태에서 남은 트리거가 다음 상태에서 뒤늦게 재생되지 않게 지운다.
            _animator.ResetTrigger(s_attackHash);
        }

        public float DistanceToPlayer()
        {
            return Vector3.Distance(transform.position, _player.transform.position);
        }

        public float DistanceFromStart()
        {
            return Vector3.Distance(transform.position, _startPosition);
        }

        public bool IsPlayerDetected()
        {
            return DistanceToPlayer() <= _detectDistance;
        }

        // 높이는 무시하고 바닥 기준으로 도착했는지 본다.
        public bool IsArrived(Vector3 target)
        {
            Vector3 offset = target - transform.position;
            offset.y = 0f;
            return offset.magnitude < _arriveDistance;
        }

        public void MoveTo(Vector3 target)
        {
            _agent.SetDestination(target);
        }

        public void LookAtPlayer()
        {
            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
                transform.forward = direction.normalized;
        }

        public void TryAttackPlayer()
        {
            if (_attackTimer > 0f)
                return;

            _attackTimer = _attackInterval;
            _player.TakeDamage(new DamageInfo(_damage));
            if (_animator != null)
                _animator.SetTrigger(s_attackHash);
        }

        public void RestoreHealth()
        {
            Health = _maxHealth;
            OnHealthChanged?.Invoke();
        }

        public void SetBodyEnabled(bool isEnabled)
        {
            _agent.enabled = isEnabled;
            if (_collider != null)
                _collider.enabled = isEnabled;
        }

        public void ResetAttackTimer()
        {
            _attackTimer = 0f;
        }

        public void Despawn()
        {
            if (_pool != null)
                _pool.Release(this);
            else
                Destroy(gameObject);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_state == MonsterState.Death)
                return;

            Health = Mathf.Max(0f, Health - damageInfo.Value);
            OnHealthChanged?.Invoke();

            SpawnBloodSpray(damageInfo);

            if (Health > 0f)
            {
                ChangeState(MonsterState.Hit);
                if (_animator != null)
                    _animator.SetTrigger(s_hitHash);
            }
            else
            {
                ChangeState(MonsterState.Death);
                OnDeath?.Invoke(this);
            }
        }

        private void SpawnBloodSpray(DamageInfo damageInfo)
        {
            if (_bloodSprayPrefab == null)
                return;

            Quaternion rotation = damageInfo.HitNormal == Vector3.zero
                ? Quaternion.identity
                : Quaternion.LookRotation(damageInfo.HitNormal);
            ParticleSystem bloodSpray = Instantiate(_bloodSprayPrefab, damageInfo.HitPoint, rotation, transform);

            // 좀비는 풀에서 재사용되므로, 자식으로 붙은 피 이펙트가 쌓이지 않게 다 뿜고 나면 지운다.
            ParticleSystem.MainModule main = bloodSpray.main;
            Destroy(bloodSpray.gameObject, main.duration + main.startLifetime.constantMax);
        }

        public void Stop()
        {
            // 풀에서 꺼지기 전이나 죽어서 에이전트가 꺼진 상태면 ResetPath가 에러를 내므로 건너뛴다.
            if (!_agent.isActiveAndEnabled || !_agent.isOnNavMesh)
                return;

            _agent.ResetPath();
            _agent.velocity = Vector3.zero;
        }
    }
}
