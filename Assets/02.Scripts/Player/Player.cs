using System;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private PlayerStats _stats;

        private void Awake()
        {
            _stats = GetComponent<PlayerStats>();
        }

        public bool TryTakeDamage(float damage)
        {
            if (_stats.Health <= 0f)
            {
                return false;
            }

            _stats.SetHealth(_stats.Health - damage);
            return true;
        }
    }
}
