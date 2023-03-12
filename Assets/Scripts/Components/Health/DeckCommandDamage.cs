using Cysharp.Threading.Tasks;
using Deck.Agent;
using Deck.Components.Operations;
using Deck.Data.Damage;

namespace Deck.Components
{
    public class DeckCommandDamage : DeckCommand
    {
        private bool _canKill;
        private DeckAgent _target;
        private DeckDataDamage _damageData;

        public DeckCommandDamage(DeckDataDamage damageData, DeckAgent target, bool canKill = true)
        {
            commandType = DeckCommandType.TakeDamage;
            _damageData = damageData;
            _canKill = canKill;
            _target = target;
        }

        public override UniTask<bool> ProcessCommand()
        {
            _target.GetDeckComponent<DeckHealthComponent>().ChangeHealth(_damageData, _canKill);
            return default;
        }
    }
}