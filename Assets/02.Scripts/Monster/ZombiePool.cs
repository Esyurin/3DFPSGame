using UnityEngine;
using UnityEngine.Pool;

namespace Monster
{
    // 좀비 오브젝트 풀: 죽은 좀비를 파괴하지 않고 꺼 두었다가 다시 꺼내 쓴다.
    public class ZombiePool : MonoBehaviour
    {
        [SerializeField] private Zombie _zombiePrefab;
        [SerializeField] private int _initialSize = 5;
        [SerializeField] private int _maxSize = 20;

        private ObjectPool<Zombie> _pool;

        private void Awake()
        {
            _pool = new ObjectPool<Zombie>(
                CreateZombie,
                actionOnRelease: zombie => zombie.gameObject.SetActive(false),
                actionOnDestroy: zombie => Destroy(zombie.gameObject),
                defaultCapacity: _initialSize,
                maxSize: _maxSize);

            // 게임 중에 멈칫하지 않도록 미리 만들어 둔다.
            Zombie[] zombies = new Zombie[_initialSize];
            for (int i = 0; i < _initialSize; i++)
                zombies[i] = _pool.Get();

            for (int i = 0; i < _initialSize; i++)
                _pool.Release(zombies[i]);
        }

        public Zombie Get(Vector3 position, Transform[] waypoints)
        {
            Zombie zombie = _pool.Get();
            zombie.Spawn(position, waypoints);
            zombie.gameObject.SetActive(true);
            return zombie;
        }

        public void Release(Zombie zombie)
        {
            _pool.Release(zombie);
        }

        private Zombie CreateZombie()
        {
            Zombie zombie = Instantiate(_zombiePrefab, transform);
            zombie.gameObject.SetActive(false);
            zombie.Init(this);
            return zombie;
        }

        private void OnDestroy()
        {
            _pool.Clear();
        }
    }
}
