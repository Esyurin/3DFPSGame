using System;
using Monster;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ZombieHpBar : MonoBehaviour
    {
        private Slider _slider;
        private Zombie _zombie;
        private Camera _camera;

        private void Awake()
        {
            _slider = GetComponent<Slider>();
            _zombie = GetComponentInParent<Zombie>();
            _camera = Camera.main;
        }

        private void Start()
        {
            _slider.value = _zombie.Health / _zombie.MaxHealth;
        }

        private void OnEnable()
        {
            _zombie.OnHealthChanged += UpdateHpBar;

            // 풀에서 다시 꺼낸 좀비는 꺼져 있는 동안 체력이 바뀌었으므로 다시 맞춘다.
            UpdateHpBar();
        }

        private void OnDisable()
        {
            _zombie.OnHealthChanged -= UpdateHpBar;
        }

        private void LateUpdate()
        {
            // 빌보드: 체력바가 카메라를 쳐다보게 한다.
            transform.LookAt(_camera.transform);
        }

        private void UpdateHpBar()
        {
            // 0 ~ 100% -> 0 ~ 1
            // _slider.vale = 좀비의 현재 체력 / 좀비의 최대 체력
            _slider.value = _zombie.Health / _zombie.MaxHealth;
        }
    }
}
