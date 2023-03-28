using Deck.Data.Item;
using Deck.Events;
using Deck.UI.Health;
using TMPro;
using UnityEngine;
using Zenject;

namespace Deck.UI.Item
{
    public class DeckUIItemDisplayer : DeckUIWorldDisplay, IPoolable<IMemoryPool>
    {
        [SerializeField] private RectTransform backGround;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Vector2 padding;
        private IMemoryPool _memory;

        public void SetData(DeckDataItem item)
        {
            if (item.Amount == 1)
            {
                label.text = item.Name;
            }
            else
            {
                label.text = item.Name + " X" + item.Amount;
            }


            var width = label.preferredWidth < label.rectTransform.sizeDelta.x ? label.preferredWidth : label.rectTransform.sizeDelta.x;
            var size = new Vector2(width + padding.x, label.preferredHeight + padding.y);
            backGround.sizeDelta = size;
        }

        public void OnDespawned()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().RemoveDisplay(this);
            _memory = null;
        }

        public void Despawn()
        {
            _memory.Despawn(this);
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _memory = p1;
            Deck.GetService<DeckServiceUI>().GetUI<DeckUIWorldLabelDisplayer>().AddDisplay(this);
        }

        public class Factory : PlaceholderFactory<DeckUIItemDisplayer>
        {
        }
    }
}