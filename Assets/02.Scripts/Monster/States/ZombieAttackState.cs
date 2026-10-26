namespace Monster.States
{
    // 공격: 플레이어를 바라보며 공격 간격마다 때린다.
    public class ZombieAttackState : ZombieState
    {
        public ZombieAttackState(Zombie zombie) : base(zombie)
        {
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
