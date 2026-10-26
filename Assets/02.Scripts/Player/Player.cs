using System;
using Core;
using UI;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private HitEffectUI _hitEffectUI;

        private PlayerStats _stats;

        public event Action OnDamaged;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_stats.Health <= 0f)
                return;
            OnDamaged?.Invoke();
            _stats.SetHealth(_stats.Health - damageInfo.Value);
        }
    }
}
