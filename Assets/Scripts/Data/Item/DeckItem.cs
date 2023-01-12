using UnityEngine;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Item", menuName = "Deck/Item", order = 0)]
    public class DeckItem : ScriptableObject
    {
        public new string name;
        public Sprite icon;
        public int amount;
    }
}