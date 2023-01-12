using UnityEngine;

namespace Deck.Data.Damage
{
    [CreateAssetMenu(fileName = "Deck Damage Data", menuName = "Deck/Component/Damage", order = 0)]
    public class DeckDamageData : ScriptableObject
    {
        public int damageAmount;
    }
}