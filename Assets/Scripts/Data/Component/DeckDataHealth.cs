using System.Linq;
using Data.Component;
using UnityEngine;

namespace Deck.Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Health", menuName = "Deck/Data/Component/Health")]
    public class DeckDataHealth : DeckDataComponent
    {
        [SerializeField] private int health;
        [SerializeField] private string[] tags;
        public int Health => health;
        public string[] GetTags() => tags;
        public bool HasTag(string tag) => tags.Contains(tag);
    }
}