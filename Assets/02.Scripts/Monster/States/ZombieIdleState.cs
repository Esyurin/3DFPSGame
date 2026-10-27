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
            // 복귀·배회 중에 남은 경로를 따라 계속 걸어가지 않게 멈춘다.
            Zombie.Stop();
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
