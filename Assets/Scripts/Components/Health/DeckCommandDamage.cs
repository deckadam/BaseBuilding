using Deck.Components.Operations;
using Deck.Data.Damage;

namespace Deck.Components
{
    public class DeckCommandDamage : DeckCommand
    {
        public bool canKill { get; }
        public DeckDataDamage damageData { get; }

        public DeckCommandDamage(DeckDataDamage damageData, bool canKill = true)
        {
            commandType = DeckCommandType.TakeDamage;
            this.damageData = damageData;
            this.canKill = canKill;
        }
    }
}