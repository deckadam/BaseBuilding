using UnityEngine;

namespace Deck.Data.Item
{
    [CreateAssetMenu(fileName = "Deck Data Item Tags", menuName = "Deck/Data/Item Tags", order = 1)]
    public class DeckDataItemTag : ScriptableObject
    {
        [SerializeField] private string[] tags;

        public string[] Tags => tags;
    }
}