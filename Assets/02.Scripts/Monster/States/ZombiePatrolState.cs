using UnityEngine;

namespace Monster.States
{
    // 순찰: 웨이포인트를 순서대로 돈다.
    public class ZombiePatrolState : ZombieState
    {
        private int _index;

        public ZombiePatrolState(Zombie zombie) : base(zombie)
        {
        }

        public override void Enter()
        {
            // 추적하다 돌아왔을 수도 있으니, 가장 가까운 웨이포인트부터 다시 돈다.
            _index = FindNearestIndex();
        }

        public override void Update()
        {
            if (Zombie.IsPlayerDetected())
            {
                Zombie.ChangeState(MonsterState.Trace);
                return;
            }

            Vector3 target = Zombie.Waypoints[_index].position;
            if (Zombie.IsArrived(target))
            {
                _index = (_index + 1) % Zombie.Waypoints.Length;
                return;
            }

            Zombie.MoveTo(target);
        }

        private int FindNearestIndex()
        {
            int nearestIndex = 0;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < Zombie.Waypoints.Length; i++)
            {
                float distance = Vector3.Distance(Zombie.transform.position, Zombie.Waypoints[i].position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIndex = i;
                }
            }

            return nearestIndex;
        }
    }
}
