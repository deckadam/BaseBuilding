using UnityEngine;

namespace Data.Agent
{
    [CreateAssetMenu(fileName = "Deck Data Damage", menuName = "Deck/Data/Component/Damage", order = 0)]
    public class DeckDataDamage : DeckDataAgent
    {
        [SerializeField] private int damageAmount;
        [SerializeField] private float attackRange;
        [SerializeField] private int attackDuration;

        public int GetDamageAmount() => damageAmount;
        public float GetAttackRange() => attackRange;
        public int GetAttackDuration() => attackDuration;
    }
}