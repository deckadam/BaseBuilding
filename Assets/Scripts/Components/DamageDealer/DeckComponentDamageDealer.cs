using System;
using Cysharp.Threading.Tasks;
using Deck;
using Deck.Data.Damage;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentDamageDealer : DeckComponent
    {
        public Action<int> OnDamageDealRequested;

        private DeckDataDamage _dataDamage;
        private bool _canAttack;

        protected override void InternalPreInitialize()
        {
            _dataDamage = holder.GetData<DeckDataDamage>();
        }

        public void DealDamage(DeckAgent target)
        {
            var healthComponent = target.GetDeckComponent<DeckComponentHealth>();

            if (healthComponent == null)
            {
                return;
            }

            if (healthComponent.GetComponentHolder() == holder)
            {
                return;
            }

            holder.AddCommand(new DeckCommandDealDamage(GetFinalDamageValue(),
                _dataDamage.GetAttackRange(),
                this,
                healthComponent,
                () => OnDamageDealRequested?.Invoke(_dataDamage.GetAttackDuration()),
                continuous: true));
        }

        public bool CanAttack() => _canAttack;

        public async void OnAttack()
        {
            _canAttack = false;
            await UniTask.Delay(_dataDamage.GetAttackDuration());
            _canAttack = true;
        }

        //Modifiers will apply here
        private int GetFinalDamageValue()
        {
            return _dataDamage.GetDamageAmount();
        }
    }
}