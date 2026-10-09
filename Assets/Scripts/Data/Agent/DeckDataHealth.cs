using System.Collections.Generic;
using UnityEngine;

namespace Data.Agent
{
    [CreateAssetMenu(fileName = "Deck Data Health", menuName = "Deck/Data/Component/Health")]
    public class DeckDataHealth : DeckDataAgent
    {
        [SerializeField] private int health;
        [SerializeField] private List<DeckActionTag> tags;
        public int Health => health;
        public List<DeckActionTag> GetTags() => tags;
        public bool HasTag(DeckActionTag tag) => this.tags.Contains(tag);
    }
}