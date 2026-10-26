namespace Monster.States
{
    // 추적: 플레이어를 쫓아간다. 시작 지점에서 너무 멀어지면 돌아간다.
    public class ZombieTraceState : ZombieState
    {
        public ZombieTraceState(Zombie zombie) : base(zombie)
        {
        }

        public override void Update()
        {
            if (Zombie.DistanceFromStart() > Zombie.ComebackDistance)
            {
                Zombie.ChangeState(MonsterState.Comeback);
                return;
            }

            if (Zombie.DistanceToPlayer() <= Zombie.AttackDistance)
            {
                Zombie.ChangeState(MonsterState.Attack);
                return;
            }

            Zombie.MoveTo(Zombie.Player.transform.position);
        }
    }
}
