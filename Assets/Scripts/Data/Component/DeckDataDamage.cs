using Data.Component;
using UnityEngine;

namespace Deck.Data.Damage
{
    [CreateAssetMenu(fileName = "Deck Data Damage", menuName = "Deck/Data/Component/Damage", order = 0)]
    public class DeckDataDamage : DeckComponentData
    {
        public int damageAmount;
    }
}