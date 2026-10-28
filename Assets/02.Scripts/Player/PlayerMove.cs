using System;
using Game;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class MoveConfig
    {
        [SerializeField] private float _gravity = -20f;
        [SerializeField] private int _maxJumps = 2;
        [SerializeField] private float _sprintCost = 20f;
        [SerializeField] private float _jumpCost = 20f;

        public float Gravity => _gravity;
        public int MaxJumps => _maxJumps;
        public float SprintCost => _sprintCost;
        public float JumpCost => _jumpCost;
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerMove : MonoBehaviour
    {
        private static readonly int s_moveMotion = Animator.StringToHash("MoveMotion");
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _jumpPower = 10f;
        [SerializeField] private MoveConfig _config = new();

        // 코요테 타임: 발판을 살짝 벗어나도 첫 점프를 허용해 경계에서의 입력 실수를 줄인다.
        [SerializeField, Min(0f)] private float _coyoteTime = 0.1f;
        // 점프 버퍼: 착지 직전에 누른 입력을 기억해 정확한 착지 프레임을 맞출 부담을 줄인다.
        [SerializeField, Min(0f)] private float _jumpBufferTime = 0.1f;
        // 가변 점프: 키를 짧게 누르면 낮게 뛰어 장애물에 맞춰 높이를 조절할 수 있다.
        [SerializeField, Range(0f, 1f)] private float _jumpCutMultiplier = 0.5f;

        private float _yVelocity; // Vertical velocity increased by gravity on each frame
        private int _jumpCount;
        // 코요테 타임과 점프 버퍼: 실제 접지/입력 전에는 유예가 발동하지 않도록 초기화한다.
        private float _lastGroundedTime = float.NegativeInfinity;
        private float _lastJumpPressedTime = float.NegativeInfinity;
        // 가변 점프 배율이 0이어도 점프 다음 프레임의 접지 판정으로 횟수가 초기화되지 않게 한다.
        private bool _jumpedLastFrame;
        private PlayerStats _stats;
        private Animator _animator;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _stats = GetComponent<PlayerStats>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;
            Move();
        }

        private void Move()
        {
            // 1. 키보드 입력을 받는다.
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            // 2. 키보드 입력에 따라 방향을 구한다.
            Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;
            _animator.SetFloat(s_moveMotion, direction.magnitude);
            // 캐릭터의 회전 방향을 기준으로 지역 방향 벡터를 월드 방향 벡터로 변환한다.
            direction = transform.TransformDirection(direction);

            float moveSpeed = _stats.MoveSpeed;
            if (Input.GetButton("Dash"))
            {
                moveSpeed = _stats.Stamina > 0f ? _stats.SprintSpeed : _stats.MoveSpeed;
                _stats.SetStamina(_stats.Stamina - _config.SprintCost * Time.deltaTime);
            }

            // 접지 가드: 점프 직후 남은 접지 판정이 _jumpCount를 0으로 되돌려 추가 점프를 허용하지 않게 한다.
            bool isGrounded = _characterController.isGrounded && _yVelocity <= 0f && !_jumpedLastFrame;
            if (isGrounded)
            {
                _jumpCount = 0;
                _yVelocity = -1f;
                _lastGroundedTime = Time.time;
            }
            else
            {
                _yVelocity += _config.Gravity * Time.deltaTime;

                // 코요테 타임: 유예가 끝나면 첫 점프 기회만 만료시키고 공중 점프 1회는 남긴다.
                if (_jumpCount == 0 && Time.time - _lastGroundedTime > _coyoteTime)
                {
                    _jumpCount = 1;
                }
            }

            // 점프 버퍼: 누른 순간을 기억해 착지나 스태미나 회복 직후에도 입력을 이어 준다.
            if (Input.GetButtonDown("Jump"))
            {
                _lastJumpPressedTime = Time.time;
            }

            _jumpedLastFrame = false;
            if (Time.time - _lastJumpPressedTime <= _jumpBufferTime &&
                _jumpCount < _config.MaxJumps && _stats.Stamina > _config.JumpCost)
            {
                _yVelocity = _jumpPower;
                _jumpCount++;
                _stats.SetStamina(_stats.Stamina - _config.JumpCost);
                _jumpedLastFrame = true;
                // 점프 버퍼는 성공할 때만 소모하며, 스태미나 부족 시에는 만료까지 유지한다.
                // 코요테 유예도 함께 끝내 첫 점프와 공중 점프가 중복 허용되지 않게 한다.
                _lastJumpPressedTime = float.NegativeInfinity;
                _lastGroundedTime = float.NegativeInfinity;
            }

            // 가변 점프: 첫 점프와 2단 점프 모두 상승 중 키를 떼면 일찍 상승을 끊어 높이를 조절한다.
            // 버퍼 발동 전에 키를 뗀 경우도 같은 프레임에 컷하며, 키 해제와 겹쳐도 한 번만 적용한다.
            if ((Input.GetButtonUp("Jump") || (_jumpedLastFrame && !Input.GetButton("Jump"))) && _yVelocity > 0f)
            {
                _yVelocity *= _jumpCutMultiplier;
            }

            Vector3 velocity = direction * moveSpeed + Vector3.up * _yVelocity;
            _characterController.Move(velocity * Time.deltaTime);
        }
    }
}
