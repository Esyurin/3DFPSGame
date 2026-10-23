using System;
using System.Collections;
using Core;
using Environment;
using Monster;
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
        [SerializeField] private int _maxBulletCount = 50;
        [SerializeField] private float _reloadTime = 1.5f;

        private Camera _camera;

        private float _nextFireTime;
        private int _bulletCount;
        private Coroutine _reloadCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _bulletCount = _maxBulletCount;
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

            if (_bulletCount <= 0)
                return;

            CancelReload();

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, _range))
                return;

            DamageInfo damageInfo = new DamageInfo(_damage, hit.point, hit.normal);
            if (hit.collider.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(damageInfo);

            _nextFireTime = Time.time + _fireCooldown;
            _bulletCount--;

            ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.identity);
            hitEffect.transform.LookAt(hit.point + hit.normal);
        }

        private void Reload()
        {
            if (_reloadCoroutine != null)
                return;

            if (_bulletCount == _maxBulletCount)
                return;

            _reloadCoroutine = StartCoroutine(ReloadRoutine());
        }

        private IEnumerator ReloadRoutine()
        {
            yield return new WaitForSeconds(_reloadTime);
            _bulletCount = _maxBulletCount;
            _reloadCoroutine = null;
        }

        private void CancelReload()
        {
            if (_reloadCoroutine == null)
                return;

            StopCoroutine(_reloadCoroutine);
            _reloadCoroutine = null;
        }
    }
}
