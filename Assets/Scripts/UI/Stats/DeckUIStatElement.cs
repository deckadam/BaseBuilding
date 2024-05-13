using TMPro;
using UnityEngine;
using Deck.Utility.Poolable;
using Zenject;

namespace Deck.UI.Stats
{
    public class DeckUIStatElement : DeckPoolable
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;

        private DeckStat _stat;

        public void SetStat(DeckStat stat)
        {
            nameText.text = stat.Name;
            valueText.text = stat.Value;
        }

        public class Factory : PlaceholderFactory<DeckUIStatElement>
        {
        }
    }
}