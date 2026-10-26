using UnityEngine;

namespace Monster.States
{
    // 대기: 잠깐 서 있다가 웨이포인트가 있으면 순찰, 없으면 배회한다.
    public class ZombieIdleState : ZombieState
    {
        private float _timer;

        public ZombieIdleState(Zombie zombie) : base(zombie)
        {
        }

        public override void Enter()
        {
            _timer = 0f;
        }

        public override void Update()
        {
            if (Zombie.IsPlayerDetected())
            {
                Zombie.ChangeState(MonsterState.Trace);
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < Zombie.IdleTime)
                return;

            Zombie.ChangeState(Zombie.HasWaypoints ? MonsterState.Patrol : MonsterState.Roaming);
        }
    }
}
