using System;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private Player.Player _player;
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _staminaSlider;

        private void Start()
        {
            _player.OnDamaged += UpdateHealth;
        }

        private void Update()
        {
            _staminaSlider.value = _stats.Stamina / _stats.MaxStamina;
        }

        private void UpdateHealth()
        {
            _healthSlider.value = _stats.Health / _stats.MaxHealth;
        }
    }
}
