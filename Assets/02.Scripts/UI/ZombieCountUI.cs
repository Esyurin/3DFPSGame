using Monster;
using TMPro;
using UnityEngine;

namespace UI
{
    // 화면 위쪽에 "남은 좀비 수 / 최대 좀비 수"를 보여 준다.
    // 매 프레임 확인하지 않고, MonsterManager가 숫자가 바뀌었다고 알릴 때만 갱신한다. (옵저버 패턴)
    public class ZombieCountUI : MonoBehaviour
    {
        private TextMeshProUGUI _countText;

        private void Awake()
        {
            _countText = GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            MonsterManager.Instance.OnCountChanged += Refresh;
            Refresh(MonsterManager.Instance.Count, MonsterManager.Instance.MaxCount);
        }

        private void OnDestroy()
        {
            if (MonsterManager.Instance != null)
                MonsterManager.Instance.OnCountChanged -= Refresh;
        }

        private void Refresh(int count, int maxCount)
        {
            _countText.SetText("Zombie: {0} / {1}", count, maxCount);
        }
    }
}
