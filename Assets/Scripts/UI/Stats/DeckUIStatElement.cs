using TMPro;
using UnityEngine;
using Deck.Components;
using Zenject;

namespace Deck.Components.Building.Stats
{
    public class DeckUIStatElement : DeckUIElement
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;

        private DeckStat _stat;

        public void SetStat(DeckStat stat)
        {
            nameText.text = stat.Name;
            valueText.text = stat.Value;
        }
    }
}