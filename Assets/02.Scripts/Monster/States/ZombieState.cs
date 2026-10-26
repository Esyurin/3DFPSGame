namespace Monster.States
{
    // 좀비 상태의 공통 틀: 상태에 들어갈 때(Enter), 매 프레임(Update), 나갈 때(Exit) 할 일을 정한다.
    public abstract class ZombieState
    {
        protected readonly Zombie Zombie;

        protected ZombieState(Zombie zombie)
        {
            Zombie = zombie;
        }

        public virtual void Enter()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Exit()
        {
        }
    }
}
