namespace Monster.States
{
    // 공격: 플레이어를 바라보며 공격 간격마다 때린다.
    public class ZombieAttackState : ZombieState
    {
        public ZombieAttackState(Zombie zombie) : base(zombie)
        {
        }

        // 공격 거리에 들어오면 바로 한 대 칠 수 있게 타이머를 비우고, 걷던 것을 멈춘다.
        public override void Enter()
        {
            Zombie.ResetAttackTimer();
            Zombie.Stop();
        }

        public override void Update()
        {
            if (Zombie.DistanceToPlayer() > Zombie.AttackDistance)
            {
                Zombie.ChangeState(MonsterState.Trace);
                return;
            }

            Zombie.LookAtPlayer();
            Zombie.TryAttackPlayer();
        }
    }
}
