using System;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float _maxHealth = 100f;
        [SerializeField] private float _healthRegen = 0f;

        [Header("Stamina")]
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _staminaRegen = 15f;

        [Header("Stats")]
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _moveSpeed = 7f;
        [SerializeField] private float _sprintSpeed = 12f;
        [SerializeField] private float _jumpPower = 7f;

        public float Health { get; private set; }
        public float Stamina { get; private set; }

        public float MaxHealth => _maxHealth;
        public float MaxStamina => _maxStamina;
        public float Damage => _damage;
        public float MoveSpeed => _moveSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float JumpPower => _jumpPower;

        private void Awake()
        {
            Health = _maxHealth;
            Stamina = _maxStamina;
        }

        private void Update()
        {
            Health = Mathf.Min(_maxHealth, Health + _healthRegen * Time.deltaTime);
            Stamina = Mathf.Min(_maxStamina, Stamina + _staminaRegen * Time.deltaTime);
        }

        public void SetHealth(float value)
        {
            Health = Mathf.Clamp(value, 0f, _maxHealth);
        }

        public void SetStamina(float value)
        {
            Stamina = Mathf.Clamp(value, 0f, _maxStamina);
        }
    }
}
