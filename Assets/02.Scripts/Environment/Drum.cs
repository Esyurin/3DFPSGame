using System.Collections;
using Core;
using UnityEngine;

namespace Environment
{
    [RequireComponent(typeof(Rigidbody))]
    public class Drum : MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem _explosionEffectPrefab;

        [Header("Stats")]
        [SerializeField] private float _health = 30f;
        [SerializeField] private float _launchPower = 10f;
        [SerializeField] private float _spinPower = 10f;
        [SerializeField] private float _destroyDelay = 3f;
        [SerializeField] private float _explosionRange = 4.5f;
        [SerializeField] private float _damage = 100f;
        [SerializeField] private float _explosionDelay = 0.2f;

        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_health <= 0)
                return;

            _health -= damageInfo.Value;

            if (_health <= 0)
                StartCoroutine(ExplodeRoutine());
        }

        private IEnumerator ExplodeRoutine()
        {
            yield return new WaitForSeconds(_explosionDelay);
            ParticleSystem effect = Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);
            effect.Play();

            _rigidbody.AddForce(Vector3.up * _launchPower, ForceMode.Impulse);
            _rigidbody.AddTorque(UnityEngine.Random.insideUnitSphere * _spinPower, ForceMode.Impulse);

            ApplyAreaDamage();

            yield return new WaitForSeconds(_destroyDelay);
            Destroy(gameObject);
        }

        private void ApplyAreaDamage()
        {
            Collider[] colliders =
                Physics.OverlapSphere(transform.position, _explosionRange, LayerMask.GetMask("Drum"));

            foreach (Collider col in colliders)
            {
                DamageInfo damageInfo = new DamageInfo(_damage);
                if (col.TryGetComponent<Drum>(out Drum drum))
                    drum.TakeDamage(damageInfo);
            }
        }
    }
}
