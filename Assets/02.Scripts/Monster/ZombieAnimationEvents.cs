using UnityEngine;

namespace Monster
{
    // 애니메이션 이벤트를 받아 부모의 Zombie에게 전해 준다.
    // 애니메이션 이벤트는 Animator가 붙은 오브젝트(Model)의 스크립트만 부를 수 있다.
    public class ZombieAnimationEvents : MonoBehaviour
    {
        private Zombie _zombie;

        private void Awake()
        {
            _zombie = GetComponentInParent<Zombie>();
        }

        // 공격 동작에서 손이 닿는 순간 불린다.
        public void AttackHit()
        {
            _zombie.OnAttackHit();
        }
    }
}
