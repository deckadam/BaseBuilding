using System;
using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.Data.Damage;
using Deck.Components;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentCommandCreator : DeckComponent
    {
        [SerializeField] private DeckDataDamage dataDamage;
        public Action<int> OnDamageDealRequested;

        private bool _canAttack = true;

        public void DealDamage(DeckAgent target)
        {
            if (!_canAttack)
            {
                return;
            }

            if (!target.TryGetDeckComponent<DeckComponentHealth>(out var healthComponent))
            {
                return;
            }

            if (healthComponent.GetAgent() == agent)
            {
                return;
            }

            if (!agent.TryGetDeckComponent<DeckComponentEquipmentManager>(out var equipmentManager))
            {
                return;
            }

            if (!agent.TryGetComponent<DeckComponentInventory>(out var inventoryComponent))
            {
                return;
            }

            if (!inventoryComponent.TryGetItemWithTag(healthComponent.GetDamagingTags(), out var requiredItem))
            {
                DeckEventNotificationRequested.Create("Can't damage this " + target.name).Send();
                return;
            }

            equipmentManager.SetItemToHold(requiredItem);

            agent.AddCommand(new DeckCommandDealDamage(GetFinalDamageValue(),
                dataDamage.GetAttackRange(),
                this,
                healthComponent,
                continuous: true));
        }

        public void OnAttackStart()
        {
            OnDamageDealRequested?.Invoke(dataDamage.GetAttackDuration());
        }

        public bool CanAttack() => _canAttack;

        public async void OnAttack()
        {
            _canAttack = false;
            await UniTask.Delay(dataDamage.GetAttackDuration());
            _canAttack = true;
        }

        //Modifiers will apply here
        private int GetFinalDamageValue()
        {
            return dataDamage.GetDamageAmount();
        }
    }
}