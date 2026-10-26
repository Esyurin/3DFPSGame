using Game;
using UnityEngine;

namespace Monster
{
    // 좀비 스포너: 일정 간격마다 랜덤한 스폰 위치에서 좀비를 하나씩 꺼낸다. 필드가 꽉 차면 쉰다.
    public class ZombieSpawner : MonoBehaviour
    {
        [SerializeField] private ZombiePool _zombiePool;
        [SerializeField] private ZombieSpawnPoint[] _spawnPoints;
        [SerializeField] private float _spawnInterval = 3f;

        private float _timer;

        private void Awake()
        {
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                Debug.LogError($"{name}: 스폰 위치가 없습니다.");
                enabled = false;
                return;
            }

            // 게임이 시작되면 첫 좀비는 바로 나오게 한다.
            _timer = _spawnInterval;
        }

        private void Update()
        {
            if (GameManager.Instance.State != GameState.Playing)
                return;

            if (MonsterManager.Instance.IsFull)
                return;

            _timer += Time.deltaTime;
            if (_timer < _spawnInterval)
                return;

            _timer = 0f;
            Spawn();
        }

        private void Spawn()
        {
            ZombieSpawnPoint spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            Zombie zombie = _zombiePool.Get(spawnPoint.Position, spawnPoint.Waypoints);
            MonsterManager.Instance.Register(zombie);
        }
    }
}
