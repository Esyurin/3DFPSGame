using System;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledEffect : MonoBehaviour
    {
        private Action<PooledEffect> _release; // 다 재생되면 자기를 풀에 돌려준다.
        private ParticleSystem _particleSystem;
        private bool _isPlaying;

        public void Init(Action<PooledEffect> release)
        {
            _release = release;
            _particleSystem = GetComponent<ParticleSystem>();
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public void Play()
        {
            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play(true);
            _isPlaying = true;
        }

        private void Update()
        {
            if (!_isPlaying || _particleSystem.IsAlive(true))
                return;

            _isPlaying = false;
            _release(this);
        }
    }
}
