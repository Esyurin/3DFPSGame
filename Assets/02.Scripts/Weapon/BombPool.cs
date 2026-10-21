using UnityEngine;
using UnityEngine.Pool;

namespace Weapon
{
    public class BombPool : MonoBehaviour
    {
        [SerializeField] private Bomb _bombPrefab;
        [SerializeField] private PooledEffect _effectPrefab;
        [SerializeField] private int _initialSize = 5;
        [SerializeField] private int _maxSize = 10;

        private ObjectPool<Bomb> _bombPool;
        private ObjectPool<PooledEffect> _effectPool;

        private void Awake()
        {
            _bombPool = new ObjectPool<Bomb>(
                CreateBomb,
                actionOnRelease: bomb =>
                {
                    bomb.ResetState();
                    bomb.gameObject.SetActive(false);
                },
                actionOnDestroy: bomb => Destroy(bomb.gameObject),
                defaultCapacity: _initialSize,
                maxSize: _maxSize);

            _effectPool = new ObjectPool<PooledEffect>(
                CreateEffect,
                actionOnRelease: effect => effect.gameObject.SetActive(false),
                actionOnDestroy: effect => Destroy(effect.gameObject),
                defaultCapacity: _initialSize,
                maxSize: _maxSize);

            Bomb[] bombs = new Bomb[_initialSize];
            PooledEffect[] effects = new PooledEffect[_initialSize];
            for (int i = 0; i < _initialSize; i++)
            {
                bombs[i] = _bombPool.Get();
                effects[i] = _effectPool.Get();
            }

            for (int i = 0; i < _initialSize; i++)
            {
                _bombPool.Release(bombs[i]);
                _effectPool.Release(effects[i]);
            }
        }

        public Bomb GetBomb(Vector3 position)
        {
            Bomb bomb = _bombPool.Get();
            bomb.transform.SetPositionAndRotation(position, Quaternion.identity);
            bomb.gameObject.SetActive(true);
            return bomb;
        }

        public PooledEffect GetEffect(Vector3 position)
        {
            PooledEffect effect = _effectPool.Get();
            effect.transform.SetPositionAndRotation(position, Quaternion.identity);
            effect.gameObject.SetActive(true);
            return effect;
        }

        public void ReleaseBomb(Bomb bomb)
        {
            _bombPool.Release(bomb);
        }

        public void ReleaseEffect(PooledEffect effect)
        {
            _effectPool.Release(effect);
        }

        private Bomb CreateBomb()
        {
            Bomb bomb = Instantiate(_bombPrefab, transform);
            bomb.gameObject.SetActive(false);
            bomb.Init(this);
            return bomb;
        }

        private PooledEffect CreateEffect()
        {
            PooledEffect effect = Instantiate(_effectPrefab, transform);
            effect.gameObject.SetActive(false);
            effect.Init(this);
            return effect;
        }

        private void OnDestroy()
        {
            _bombPool.Clear();
            _effectPool.Clear();
        }
    }
}
