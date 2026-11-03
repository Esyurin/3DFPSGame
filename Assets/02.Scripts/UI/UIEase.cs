using UnityEngine;

namespace UI
{
    // UI 연출에 쓰는 이징 함수. t는 0~1.
    public static class UIEase
    {
        public static float OutCubic(float t)
        {
            t = 1f - Mathf.Clamp01(t);
            return 1f - t * t * t;
        }

        public static float InOutCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
        }

        // 목표를 살짝 지나쳤다가 돌아온다.
        public static float OutBack(float t)
        {
            t = Mathf.Clamp01(t);
            const float Overshoot = 1.70158f;
            const float OvershootPlusOne = Overshoot + 1f;
            return 1f + OvershootPlusOne * Mathf.Pow(t - 1f, 3f) + Overshoot * Mathf.Pow(t - 1f, 2f);
        }
    }
}
