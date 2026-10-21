using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class BombAmountUI : MonoBehaviour
    {
        private TextMeshProUGUI _bombAmountText;

        private void Awake()
        {
            _bombAmountText = GetComponent<TextMeshProUGUI>();
        }

        public void Refresh(int amount)
        {
            _bombAmountText.SetText("Bomb: {0}", amount);
        }
    }
}