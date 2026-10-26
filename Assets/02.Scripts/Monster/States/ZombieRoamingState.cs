using UnityEngine;

namespace Monster.States
{
    // 배회: 시작 지점 주변의 랜덤한 곳으로 걸어간 뒤 다시 대기한다.
    public class ZombieRoamingState : ZombieState
    {
        private Vector3 _target;
        private float _timer;

        public ZombieRoamingState(Zombie zombie) : base(zombie)
        {
        }

        public override void Enter()
        {
            Vector2 offset = Random.insideUnitCircle * Zombie.RoamRadius;
            _target = Zombie.StartPosition + new Vector3(offset.x, 0f, offset.y);
            _timer = 0f;
        }

        public override void Update()
        {
            if (Zombie.IsPlayerDetected())
            {
                Zombie.ChangeState(MonsterState.Trace);
                return;
            }

            // 벽에 막혀 도착하지 못해도 너무 오래 걷지 않게 한다.
            _timer += Time.deltaTime;
            if (Zombie.IsArrived(_target) || _timer >= Zombie.RoamTimeout)
            {
                Zombie.ChangeState(MonsterState.Idle);
                return;
            }

            Zombie.MoveTo(_target);
        }
    }
}
