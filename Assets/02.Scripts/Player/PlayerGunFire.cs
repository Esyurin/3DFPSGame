using System;
using System.Collections;
using Core;
using Environment;
using Game;
using Monster;
using UnityEngine;
using UnityEngine.EventSystems;

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

        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLine; // 총을 쏠 때 잠깐 보이는 레이저 선
        [SerializeField] private Transform _muzzle; // 레이저가 시작되는 총구 위치
        [SerializeField] private float _laserDuration = 0.05f; // 레이저가 보이는 시간 (s)

        private Camera _camera;

        private float _nextFireTime;
        private int _bulletCount;
        private Coroutine _reloadCoroutine;
        private Coroutine _laserCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _bulletCount = _maxBulletCount;
            _laserLine.positionCount = 2;
            _laserLine.enabled = false;
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

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

            // 2. 버튼을 누르고 있고, 마우스가 UI 위에 있지 않고, 쏠 때가 되었고, 총알이 남아 있으면 쏜다.
            bool isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
            if (isPointerOverUI)
                return;

            CancelReload();

            // 빗나가도 쏜 것이므로 총알과 쿨다운은 먼저 쓴다.
            _nextFireTime = Time.time + _fireCooldown;
            _bulletCount--;

            // 빗나가면 사거리 끝까지 레이저를 그린다.
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            Vector3 endPoint = ray.origin + ray.direction * _range;

            if (Physics.Raycast(ray, out RaycastHit hit, _range))
            {
                endPoint = hit.point;

                DamageInfo damageInfo = new DamageInfo(_damage, hit.point, hit.normal);
                if (hit.collider.TryGetComponent(out IDamageable damageable))
                    damageable.TakeDamage(damageInfo);

                ParticleSystem hitEffect = Instantiate(_hitEffectPrefab, hit.point, Quaternion.identity);
                hitEffect.transform.LookAt(hit.point + hit.normal);
            }

            ShowLaser(_muzzle.position, endPoint);
        }

        private void ShowLaser(Vector3 start, Vector3 end)
        {
            _laserLine.SetPosition(0, start);
            _laserLine.SetPosition(1, end);
            _laserLine.enabled = true;

            // 연사 중이면 이전 레이저를 끄는 예약을 취소하고 새로 잡는다.
            if (_laserCoroutine != null)
                StopCoroutine(_laserCoroutine);
            _laserCoroutine = StartCoroutine(HideLaserRoutine());
        }

        private IEnumerator HideLaserRoutine()
        {
            yield return new WaitForSeconds(_laserDuration);
            _laserLine.enabled = false;
            _laserCoroutine = null;
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
