using Player;
using UnityEngine;

namespace Weapon
{
    // 칼날(트리거)에 닿은 것을 Player의 PlayerSwordSwing에게 알려 준다.
    // 트리거 이벤트는 콜라이더가 붙은 오브젝트(Blade)의 스크립트가 받는다.
    public class MeleeWeapon : MonoBehaviour
    {
        private PlayerSwordSwing _swordSwing;

        private void Awake()
        {
            _swordSwing = GetComponentInParent<PlayerSwordSwing>();
        }

        private void OnTriggerEnter(Collider other)
        {
            _swordSwing.Hit(other);
        }
    }
}
