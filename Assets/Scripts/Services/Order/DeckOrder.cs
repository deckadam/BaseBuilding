using Data.Currency;
using UnityEngine;

namespace Services.Order
{
    [CreateAssetMenu(fileName = "Deck Order",menuName = "Deck/Data/Order")]
    public class DeckOrder : ScriptableObject
    {
        [SerializeField] private DeckActionTag requiredAgentTag;
        [SerializeField] private DeckOrderType orderType;
        [SerializeField] private int amount;
        [SerializeField] private DeckPrice[] price;

        public DeckActionTag RequiredAgentTag => requiredAgentTag;
        public DeckOrderType OrderType => orderType;
        public int Amount => amount;
        public DeckPrice[] Price => price;
    }
}