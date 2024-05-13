using System.Collections.Generic;
using Data.Component;
using UnityEngine;

namespace Deck.Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Health", menuName = "Deck/Data/Component/Health")]
    public class DeckDataHealth : DeckDataComponent
    {
        [SerializeField] private int health;
        [SerializeField] private List<DeckActionTag> tags;
        public int Health => health;
        public List<DeckActionTag> GetTags() => tags;
        public bool HasTag(DeckActionTag tag) => this.tags.Contains(tag);
    }
}