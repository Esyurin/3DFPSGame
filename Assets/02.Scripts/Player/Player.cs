using System;
using UI;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private HitEffectUI _hitEffectUI;

        private PlayerStats _stats;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        public bool TryTakeDamage(float damage)
        {
            if (_stats.Health <= 0f)
            {
                return false;
            }

            _stats.SetHealth(_stats.Health - damage);
            _hitEffectUI.Play();
            return true;
        }
    }
}
