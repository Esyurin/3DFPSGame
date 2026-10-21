using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bomb : MonoBehaviour
    {
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
            PooledEffect explosionEffect = _bombPool.GetEffect(collision.GetContact(0).point);
            explosionEffect.Play();
            _bombPool.ReleaseBomb(this);
        }
    }
}
