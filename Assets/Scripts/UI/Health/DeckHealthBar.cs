using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Health
{
    public class DeckHealthBar : DeckUIWorldDisplay
    {
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private Image fillBar;

        private IMemoryPool _memory;

        public void OnDataChanged(int value, float ratio)
        {
            numberDisplay.text = value.ToString();
            fillBar.fillAmount = ratio;
        }
    }
}