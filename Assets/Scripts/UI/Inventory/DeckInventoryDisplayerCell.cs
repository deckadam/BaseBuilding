using Deck.Data.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckInventoryDisplayerCell : MonoBehaviour, IPoolable<IMemoryPool>
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI amount;

        private IMemoryPool _pool;

        public void Initialize(DeckItem item)
        {
            image.sprite = item.icon;
            amount.text = item.amount.ToString();
        }

        public class Factory : PlaceholderFactory<DeckInventoryDisplayerCell>
        {
        }

        public void Despawn()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }
    }
}