using UnityEngine;

namespace Core
{
    public struct DamageInfo
    {
        public float Value; // 대미지 양
        public Vector3 HitPoint; // 맞은 자리
        public Vector3 HitNormal;

        public DamageInfo(float value)
        {
            Value = value;
            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
        }

        public DamageInfo(float value, Vector3 hitPoint, Vector3 hitNormal)
        {
            Value = value;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
        }
    }
}
