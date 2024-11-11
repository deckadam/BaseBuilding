using TMPro;
using UnityEngine;
using Deck.Components;

namespace Deck.UI.Stats
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