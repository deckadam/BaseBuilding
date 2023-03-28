using Data.Component;
using UnityEngine;

namespace Deck.Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Health", menuName = "Deck/Data/Component/Health")]
    public class DeckDataHealth : DeckDataComponent
    {
        [SerializeField] private int health;
        public int Health => health;
    }
}