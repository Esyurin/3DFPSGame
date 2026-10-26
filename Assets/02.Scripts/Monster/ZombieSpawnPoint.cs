using UnityEngine;

namespace Monster
{
    // 좀비 스폰 위치: 웨이포인트를 넣으면 여기서 나온 좀비는 순찰하고, 비워 두면 배회한다.
    public class ZombieSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform[] _waypoints;

        public Vector3 Position => transform.position;
        public Transform[] Waypoints => _waypoints;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            if (_waypoints == null || _waypoints.Length == 0)
                return;

            // 순찰 경로를 선으로 이어서 보여 준다.
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _waypoints.Length; i++)
            {
                Transform from = _waypoints[i];
                Transform to = _waypoints[(i + 1) % _waypoints.Length];
                if (from != null && to != null)
                    Gizmos.DrawLine(from.position, to.position);
            }
        }
    }
}
