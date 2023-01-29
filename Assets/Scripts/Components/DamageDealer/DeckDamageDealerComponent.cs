using System;
using Deck.Components.Operations;
using Deck.Data.Damage;

namespace Deck.Components
{
    [Serializable]
    public class DeckDamageDealerComponent : DeckComponent
    {
        private DeckDataDamage _dataDamage;

        protected override void Initialize()
        {
            _dataDamage = holder.GetData<DeckDataDamage>();
        }

        private void DealDamage(DeckCommand command)
        {
        }

        public override DeckCommandListener[] GetSupportedCommandTypes()
        {
            return new[] {DeckCommandListener.Create(DeckCommandType.DealDamage, DealDamage)};
        }
    }
}