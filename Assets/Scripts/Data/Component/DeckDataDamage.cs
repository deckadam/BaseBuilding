using UnityEngine;

namespace Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Damage", menuName = "Deck/Data/Component/Damage", order = 0)]
    public class DeckDataDamage : DeckDataComponent
    {
        [SerializeField] private int damageAmount;
        [SerializeField] private float attackRange;
        [SerializeField] private int attackDuration;

        public int GetDamageAmount() => damageAmount;
        public float GetAttackRange() => attackRange;
        public int GetAttackDuration() => attackDuration;
    }
}