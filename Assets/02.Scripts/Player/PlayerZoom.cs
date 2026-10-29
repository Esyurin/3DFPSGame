using Game;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(PlayerWeaponMode))]
    public class PlayerZoom : MonoBehaviour
    {
        [SerializeField] private GameObject _crosshair; // 4장의 조준선
        [SerializeField] private GameObject _sniperZoom;
        [SerializeField] private float _normalFov = 60f; // 평소 시야각
        [SerializeField] private float _zoomFov = 15f; // 확대했을 때 시야각
        [SerializeField] private float _zoomSensitivityRate = 0.25f; // 확대했을 때 마우스 감도 배율

        private PlayerWeaponMode _weaponMode;
        private Camera _camera;

        public bool IsZooming { get; private set; }
        public float SensitivityRate => IsZooming ? _zoomSensitivityRate : 1f; // 시선을 돌리는 스크립트가 감도에 곱한다.

        private void Awake()
        {
            _weaponMode = GetComponentInChildren<PlayerWeaponMode>();
            _camera = Camera.main;
        }

        private void Update()
        {
            // 게임 중이고, 저격 모드이고, 오른쪽 버튼을 누르고 있을 때만 확대한다.
            IsZooming = GameManager.Instance.State == GameState.Playing
                        && _weaponMode.Mode == WeaponMode.Sniper
                        && Input.GetMouseButton(1);

            _camera.fieldOfView = IsZooming ? _zoomFov : _normalFov;
            _crosshair.SetActive(!IsZooming); // 확대하는 동안은 조준선을 숨긴다.
            _sniperZoom.SetActive(IsZooming); // 확대하는 동안만 조준경을 씌운다.
        }
    }
}
