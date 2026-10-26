namespace Monster.States
{
    // 복귀: 시작 지점으로 돌아가 체력을 회복하고 대기한다.
    public class ZombieComebackState : ZombieState
    {
        public ZombieComebackState(Zombie zombie) : base(zombie)
        {
        }

        public override void Update()
        {
            if (Zombie.IsArrived(Zombie.StartPosition))
            {
                Zombie.RestoreHealth();
                Zombie.ChangeState(MonsterState.Idle);
                return;
            }

            Zombie.MoveTo(Zombie.StartPosition);
        }
    }
}
