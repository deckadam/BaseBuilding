using Deck.Events;
using Deck.UI;
using Deck.UI.Health;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.Utility.Health
{
    public class DeckHealthBar : DeckUIWorldDisplay, IPoolable<IMemoryPool>
    {
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private Image fillBar;

        private IMemoryPool _memory;

        public void OnDataChanged(int value, float ratio)
        {
            numberDisplay.text = value.ToString();
            fillBar.fillAmount = ratio;
        }

        public void Despawn()
        {
            _memory.Despawn(this);
        }

        public void OnDespawned()
        {
            global::Deck.Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().RemoveDisplay(this);
            _memory = null;
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _memory = p1;
            global::Deck.Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().AddDisplay(this);
        }

        public class Factory : PlaceholderFactory<DeckHealthBar>
        {
        }
    }
}