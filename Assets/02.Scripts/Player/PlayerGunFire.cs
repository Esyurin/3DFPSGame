using System.Collections;
using Core;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
    // 총구가 카메라에 붙어 있으므로, 카메라가 회전·이동한 뒤에 실행되어야 총구 위치가 한 프레임 늦지 않는다.
    [DefaultExecutionOrder(100)]
    public class PlayerGunFire : MonoBehaviour
    {
        private static readonly int s_attack = Animator.StringToHash("Attack");
        private static readonly int s_moveMotion = Animator.StringToHash("MoveMotion");
        private static readonly int s_fire = Animator.StringToHash("Fire");
        private const string FireButton = "Fire1";

        [SerializeField] private ParticleSystem _hitEffectPrefab;
        [SerializeField] private ParticleSystem _muzzleFlash; // 총구에 붙어 있다가 쏠 때마다 다시 재생하는 화염

        [SerializeField] private float _range = 100f;
        [SerializeField] private float _damage = 10f;
        [SerializeField] private float _fireCooldown = 0.1f;
        [SerializeField] private int _maxBulletCount = 50;
        [SerializeField] private float _reloadTime = 1.5f;

        [Header("Laser")]
        [SerializeField] private LineRenderer _laserLine; // 총을 쏠 때 잠깐 보이는 예광탄 선
        [SerializeField] private Transform _muzzle; // 레이저가 시작되는 총구 위치
        [SerializeField] private float _laserDuration = 0.05f; // 레이저가 보이는 시간 (s)

        private Camera _camera;
        private Animator _animator;

        private float _nextFireTime;
        private int _bulletCount;
        private Coroutine _reloadCoroutine;
        private Coroutine _laserCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _animator = GetComponentInChildren<Animator>();
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

        private void LateUpdate()
        {
            // 레이저는 월드 좌표로 그리므로, 카메라가 따라간 뒤의 총구 위치로 시작점을 다시 붙인다.
            if (_laserLine.enabled)
                _laserLine.SetPosition(0, _muzzle.position);
        }

        private void Shoot()
        {
            _animator.SetBool(s_fire, false);

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

            _animator.SetBool(s_fire, true);

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

                // 피탄 이펙트가 스스로 지워지지 않아도 쌓이지 않게, 다 재생되면 지운다.
                ParticleSystem.MainModule main = hitEffect.main;
                Destroy(hitEffect.gameObject, main.duration + main.startLifetime.constantMax);
            }

            PlayMuzzleFlash();
            ShowLaser(_muzzle.position, endPoint);
        }

        private void PlayMuzzleFlash()
        {
            if (_muzzleFlash == null)
                return;

            // 연사 중에는 이전 화염이 끝나기 전에 다시 쏘므로, 지우고 처음부터 재생한다.
            _muzzleFlash.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _muzzleFlash.Play(true);
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
