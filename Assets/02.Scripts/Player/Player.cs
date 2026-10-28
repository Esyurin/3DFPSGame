using System;
using Core;
using Game;
using UI;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private HitEffectUI _hitEffectUI;

        private PlayerStats _stats;
        private Animator _animator;
        private int _injuredLayer;

        public event Action OnDamaged;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
            _animator = GetComponentInChildren<Animator>(); // 자식인 Model에 붙어 있다.
            _injuredLayer = _animator.GetLayerIndex("Injured Layer");
        }

        private void Update()
        {
            float healthRatio = _stats.Health / _stats.MaxHealth;
            float injured = Mathf.InverseLerp(0.5f, 0f, healthRatio);

            _animator.SetLayerWeight(_injuredLayer, injured);
        }

        public void TakeDamage(DamageInfo damageInfo)
        {
            if (_stats.Health <= 0f)
                return;
            OnDamaged?.Invoke();
            _stats.SetHealth(_stats.Health - damageInfo.Value);

            // 플레이어가 쓰러지면 게임 오버.
            if (_stats.Health <= 0f)
                GameManager.Instance.GameOver();
        }
    }
}
