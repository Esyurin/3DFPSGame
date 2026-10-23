using Core;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bomb : MonoBehaviour
    {
        private const float Damage = 10f;
        private const float ExplosionRange = 5f;

        private BombPool _bombPool;
        private Rigidbody _rigidbody;

        public void Init(BombPool bombPool)
        {
            _bombPool = bombPool;
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ResetState()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }

        private void OnCollisionEnter(Collision collision)
        {
            ApplyAreaDamage();
            PooledEffect explosionEffect = _bombPool.GetEffect(collision.GetContact(0).point);
            explosionEffect.Play();
            _bombPool.ReleaseBomb(this);
        }

        private void ApplyAreaDamage()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRange,
                LayerMask.GetMask("Zombie", "Drum"));

            foreach (Collider col in colliders)
            {
                Vector3 hitPoint = col.ClosestPoint(transform.position);
                Vector3 hitNormal = (transform.position - hitPoint).normalized;
                var damageInfo = new DamageInfo(Damage, hitPoint, hitNormal);
                if (col.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(damageInfo);
            }
        }
    }
}
