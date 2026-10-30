using UnityEngine;

namespace Player
{
    // Slash 동작의 애니메이션 이벤트를 받아 부모의 PlayerSwordSwing에게 전해 준다.
    // 애니메이션 이벤트는 Animator가 붙은 오브젝트(AnimeTacticalGirl)의 스크립트만 부를 수 있다.
    public class PlayerMeleeEvent : MonoBehaviour
    {
        private PlayerSwordSwing _swordSwing;

        private void Start()
        {
            _swordSwing = GetComponentInParent<PlayerSwordSwing>();
        }

        public void ActivateCollider()
        {
            _swordSwing.ActivateCollider();
        }

        public void DeactivateCollider()
        {
            _swordSwing.DeactivateCollider();
        }
    }
}
