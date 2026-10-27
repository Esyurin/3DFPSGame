using UnityEngine;

namespace Player
{
    // 플레이어의 이동 속도와 피격/사망을 캐릭터 애니메이터에 전달한다.
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerStats))]
    [RequireComponent(typeof(Player))]
    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int s_moveXHash = Animator.StringToHash("MoveX");
        private static readonly int s_moveZHash = Animator.StringToHash("MoveZ");
        private static readonly int s_hitHash = Animator.StringToHash("Hit");
        private static readonly int s_deadHash = Animator.StringToHash("Dead");

        [SerializeField] private Animator _animator;
        [SerializeField] private float _dampTime = 0.1f; // 속도 값이 바뀔 때 블렌드를 부드럽게 잇는 시간 (s)

        private CharacterController _characterController;
        private PlayerStats _stats;
        private Player _player;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _stats = GetComponent<PlayerStats>();
            _player = GetComponent<Player>();
        }

        private void OnEnable()
        {
            _player.OnDamaged += PlayHit;
        }

        private void OnDisable()
        {
            _player.OnDamaged -= PlayHit;
        }

        private void Update()
        {
            // 블렌드 트리는 기본 이동 속도를 1로 보므로, 실제 속도를 기본 이동 속도로 나눠 넘긴다.
            Vector3 localVelocity = transform.InverseTransformDirection(_characterController.velocity);
            float moveX = localVelocity.x / _stats.MoveSpeed;
            float moveZ = localVelocity.z / _stats.MoveSpeed;

            _animator.SetFloat(s_moveXHash, moveX, _dampTime, Time.deltaTime);
            _animator.SetFloat(s_moveZHash, moveZ, _dampTime, Time.deltaTime);
            _animator.SetBool(s_deadHash, _stats.Health <= 0f);
        }

        private void PlayHit()
        {
            _animator.SetTrigger(s_hitHash);
        }
    }
}
