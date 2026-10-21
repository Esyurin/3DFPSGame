using System;
using UnityEngine;

namespace Weapon
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _explosionEffectPrefab;

        private void OnCollisionEnter(Collision collision)
        {
            // TODO: Object pooling
            ParticleSystem explosionEffect =
                Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

            explosionEffect.Play();
            Destroy(gameObject);
        }
    }
}