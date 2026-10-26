using System.Collections.Generic;
using UnityEngine;

namespace Monster
{
    public class MonsterManager : MonoBehaviour
    {
        private static MonsterManager s_instance;
        public static MonsterManager Instance => s_instance;

        private readonly List<Zombie> _zombies = new List<Zombie>();

        public int Count => _zombies.Count;

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
        }

        private void Start()
        {
            GameObject[] zombieObjects = GameObject.FindGameObjectsWithTag("Zombie");

            foreach (GameObject zombieObject in zombieObjects)
            {
                if (!zombieObject.TryGetComponent(out Zombie zombie))
                    continue;

                _zombies.Add(zombie);
                zombie.OnDeath += () => HandleZombieDeath(zombie);
            }
        }

        private void OnDestroy()
        {
            if (s_instance == this)
                s_instance = null;
        }

        private void HandleZombieDeath(Zombie zombie)
        {
            _zombies.Remove(zombie);
        }
    }
}
