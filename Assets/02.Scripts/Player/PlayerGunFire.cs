using System;
using UnityEngine;

namespace Player
{
    public class PlayerGunFire : MonoBehaviour
    {
        private const string FireButton = "Fire1";

        [SerializeField] private ParticleSystem _hitEffectPrefab;

        [SerializeField] private float _range = 100f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _fireCooldown = 0.1f;
        [SerializeField] private int _magazineCapacity = 50;

        private Camera _camera;

        private float _nextFireTime;
        private int _currentAmmo;

        private void Awake()
        {
            _camera = Camera.main;
            _currentAmmo = _magazineCapacity;
        }

        private void Update()
        {
            if (Input.GetButton(FireButton))
                Shoot();

            if (Input.GetKeyDown(KeyCode.R))
                Reload();
        }

        private void Shoot()
        {
            if (Time.time < _nextFireTime)
                return;

            if (_currentAmmo <= 0)
                return;

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, _range))
                return;

            _nextFireTime = Time.time + _fireCooldown;
            _currentAmmo--;

            ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.identity);
            hitEffect.transform.LookAt(hit.point + hit.normal);
        }

        private void Reload()
        {
            _currentAmmo = _magazineCapacity;
        }
    }
}