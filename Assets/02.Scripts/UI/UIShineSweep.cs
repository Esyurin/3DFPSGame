using System.Collections;
using UnityEngine;

namespace UI
{
    // 버튼 위로 비스듬한 빛줄기가 주기적으로 지나가게 한다.
    // 부모에 RectMask2D를 달아 버튼 밖으로 나간 빛은 잘리게 한다.
    public class UIShineSweep : MonoBehaviour
    {
        [SerializeField] private RectTransform _shine;
        [SerializeField] private float _sweepDuration = 0.7f;
        [SerializeField] private float _interval = 3f;

        private RectTransform _rect;

        private void Awake()
        {
            _rect = (RectTransform)transform;
        }

        private void OnEnable()
        {
            StartCoroutine(SweepRoutine());
        }

        private IEnumerator SweepRoutine()
        {
            var wait = new WaitForSecondsRealtime(_interval);

            while (true)
            {
                yield return wait;

                // 빛줄기가 기울어져 있으므로 높이만큼 더 밀어내야 모서리가 버튼 안에 남지 않는다.
                float distance = (_rect.rect.width + _shine.rect.width + _shine.rect.height) * 0.5f;
                for (float time = 0f; time < _sweepDuration; time += Time.unscaledDeltaTime)
                {
                    float x = Mathf.Lerp(-distance, distance, UIEase.InOutCubic(time / _sweepDuration));
                    _shine.anchoredPosition = new Vector2(x, 0f);
                    yield return null;
                }

                _shine.anchoredPosition = new Vector2(distance, 0f);
            }
        }
    }
}
