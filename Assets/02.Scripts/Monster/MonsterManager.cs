using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterManager : MonoBehaviour
    {
        private static MonsterManager s_instance;
        public static MonsterManager Instance => s_instance;

        [SerializeField] private int _maxCount = 10; // 필드에 동시에 있을 수 있는 최대 좀비 수

        private readonly List<Zombie> _zombies = new List<Zombie>();

        public int Count => _zombies.Count;
        public int MaxCount => _maxCount;
        public bool IsFull => Count >= _maxCount;

        // 좀비 수가 바뀌면 (현재 수, 최대 수)를 알린다. UI 같은 구독자는 이 이벤트만 보고 갱신한다.
        public event Action<int, int> OnCountChanged;

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
        }

        private void OnDestroy()
        {
            if (s_instance == this)
                s_instance = null;
        }

        public void Register(Zombie zombie)
        {
            if (_zombies.Contains(zombie))
                return;

            _zombies.Add(zombie);
            zombie.OnDeath += HandleZombieDeath;
            NotifyCountChanged();
        }

        private void HandleZombieDeath(Zombie zombie)
        {
            // 풀에서 다시 꺼내 등록할 때 중복 구독되지 않도록 해제한다.
            zombie.OnDeath -= HandleZombieDeath;
            _zombies.Remove(zombie);
            NotifyCountChanged();
        }

        private void NotifyCountChanged()
        {
            OnCountChanged?.Invoke(Count, _maxCount);
        }
    }
}
