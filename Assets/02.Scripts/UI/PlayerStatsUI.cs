using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private PlayerStats _stats;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Slider _staminaSlider;

        private void Update()
        {
            _healthSlider.value = _stats.Health / _stats.MaxHealth;
            _staminaSlider.value = _stats.Stamina / _stats.MaxStamina;
        }
    }
}
