using Deck.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.Inventory
{
    public class DeckInventoryDisplayerCell : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI amount;

        public void Initialize(DeckItem item)
        {
            image.sprite = item.icon;
            amount.text = item.amount.ToString();
        }
    }
}