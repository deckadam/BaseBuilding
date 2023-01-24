using UnityEngine;

namespace Deck.Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Health", menuName = "Deck/Data/Component/Health")]
    public class DeckDataHealth : ScriptableObject
    {
        [SerializeField] private int health;
        
        public int Health => health;
    }
}