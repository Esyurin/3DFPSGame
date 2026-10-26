using UnityEngine;

namespace Monster.States
{
    // 피격: 잠깐 멈췄다가 플레이어를 쫓는다.
    public class ZombieHitState : ZombieState
    {
        private float _timer;

        public ZombieHitState(Zombie zombie) : base(zombie)
        {
        }

        public override void Enter()
        {
            _timer = 0f;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= Zombie.HitTime)
                Zombie.ChangeState(MonsterState.Trace);
        }
    }
}
