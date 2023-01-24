using UnityEngine;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Data Item", menuName = "Deck/Data/Item", order = 0)]
    public class DeckDataItem : ScriptableObject
    {
        public new string name;
        public Sprite icon;
        public int amount;
    }
}