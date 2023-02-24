using System;
using Cysharp.Threading.Tasks;
using Deck.Components.Operations;
using Deck.Data.Damage;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentDamageDealer : DeckComponent
    {
        private DeckDataDamage _dataDamage;

        protected override void Initialize()
        {
            _dataDamage = holder.GetData<DeckDataDamage>();
        }

        private UniTask<bool> DealDamage(DeckCommand command)
        {
            return default;
        }
    }
}