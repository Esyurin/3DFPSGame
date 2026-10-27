using UnityEngine;

namespace Monster.States
{
    // 사망: 움직임을 멈추고, 잠시 뒤 필드에서 사라진다.
    public class ZombieDeathState : ZombieState
    {
        private float _timer;

        public ZombieDeathState(Zombie zombie) : base(zombie)
        {
        }

        public override void Enter()
        {
            _timer = 0f;
            Zombie.Stop();
            Zombie.SetBodyEnabled(false);
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Zombie.DeathTime)
                Zombie.Despawn();
        }
    }
}
