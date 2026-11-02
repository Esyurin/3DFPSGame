using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapon
{
    // 피탄·피처럼 자주 나오는 짧은 파티클 이펙트를 프리팹별 풀에 담아 다시 쓴다.
    // 연사할 때마다 Instantiate/Destroy하면 GC와 멈칫함이 생기므로, 다 재생된 이펙트는 꺼 두었다가 다시 꺼낸다.
    public class EffectPool : MonoBehaviour
    {
        private static EffectPool s_instance;

        [SerializeField] private ParticleSystem[] _prewarmPrefabs; // 게임 중에 멈칫하지 않게 미리 만들어 둘 이펙트
        [SerializeField] private int _initialSize = 10; // 프리팹마다 미리 만들어 둘 수
        [SerializeField] private int _maxSize = 30; // 프리팹마다 꺼 두고 보관할 최대 수

        private readonly Dictionary<ParticleSystem, ObjectPool<PooledEffect>> _pools =
            new Dictionary<ParticleSystem, ObjectPool<PooledEffect>>();

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;

            foreach (ParticleSystem prefab in _prewarmPrefabs)
                Prewarm(prefab);
        }

        private void OnDestroy()
        {
            if (s_instance != this)
                return;

            foreach (ObjectPool<PooledEffect> pool in _pools.Values)
                pool.Clear();
            s_instance = null;
        }

        // 풀이 있으면 풀에서 꺼내 재생하고, 풀이 없는 씬이면 예전처럼 만들고 다 재생되면 지운다.
        public static void Play(ParticleSystem prefab, Vector3 position, Quaternion rotation)
        {
            if (s_instance == null)
            {
                ParticleSystem effect = Instantiate(prefab, position, rotation);
                ParticleSystem.MainModule main = effect.main;
                Destroy(effect.gameObject, main.duration + main.startLifetime.constantMax);
                return;
            }

            PooledEffect pooled = s_instance.GetPool(prefab).Get();
            pooled.transform.SetPositionAndRotation(position, rotation);
            pooled.gameObject.SetActive(true);
            pooled.Play();
        }

        private void Prewarm(ParticleSystem prefab)
        {
            ObjectPool<PooledEffect> pool = GetPool(prefab);
            PooledEffect[] effects = new PooledEffect[_initialSize];
            for (int i = 0; i < _initialSize; i++)
                effects[i] = pool.Get();

            for (int i = 0; i < _initialSize; i++)
                pool.Release(effects[i]);
        }

        private ObjectPool<PooledEffect> GetPool(ParticleSystem prefab)
        {
            if (_pools.TryGetValue(prefab, out ObjectPool<PooledEffect> pool))
                return pool;

            pool = new ObjectPool<PooledEffect>(
                () => CreateEffect(prefab),
                actionOnRelease: effect => effect.gameObject.SetActive(false),
                actionOnDestroy: effect => Destroy(effect.gameObject),
                defaultCapacity: _initialSize,
                maxSize: _maxSize);
            _pools.Add(prefab, pool);
            return pool;
        }

        private PooledEffect CreateEffect(ParticleSystem prefab)
        {
            ParticleSystem particle = Instantiate(prefab, transform);
            particle.gameObject.SetActive(false);

            // 서드파티 이펙트 프리팹을 고치지 않도록, 풀에 넣을 때 PooledEffect를 붙인다.
            if (!particle.TryGetComponent(out PooledEffect effect))
                effect = particle.gameObject.AddComponent<PooledEffect>();
            effect.Init(released => _pools[prefab].Release(released));
            return effect;
        }
    }
}
