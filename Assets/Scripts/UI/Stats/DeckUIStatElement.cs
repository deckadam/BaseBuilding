using Deck.Base;
using TMPro;
using UnityEngine;

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