using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledEffect : MonoBehaviour
    {
        private BombPool _bombPool;
        private ParticleSystem _particleSystem;
        private bool _isPlaying;

        public void Init(BombPool bombPool)
        {
            _bombPool = bombPool;
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
            _bombPool.ReleaseEffect(this);
        }
    }
}
