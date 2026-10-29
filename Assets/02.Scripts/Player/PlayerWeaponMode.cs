using Game;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerWeaponMode : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _modeText;

        private WeaponMode _mode = WeaponMode.Normal;
        public WeaponMode Mode => _mode;
        public event System.Action<WeaponMode> OnModeChanged; // 모드가 바뀌면 새 모드를 알린다.

        private void Start()
        {
            ShowMode();
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeMode(WeaponMode.Normal);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeMode(WeaponMode.Sniper);
        }

        private void ChangeMode(WeaponMode mode)
        {
            _mode = mode;
            ShowMode();
            OnModeChanged?.Invoke(mode); // 모드가 바뀌었다고 알린다.
            Debug.Log($"무기 모드: {mode}");
        }

        private void ShowMode()
        {
            _modeText.text = _mode switch
            {
                WeaponMode.Normal => "일반 모드",
                WeaponMode.Sniper => "저격 모드",
                _ => _modeText.text
            };
        }
    }
}
