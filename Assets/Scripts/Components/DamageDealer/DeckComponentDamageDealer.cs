using System;
using Base;
using Commands;
using Components.Health;
using Components.Inventory;
using Components.ItemHolder;
using Cysharp.Threading.Tasks;
using Data.Agent;
using UI.Notification;
using UnityEngine;
using Utility;

namespace Components.DamageDealer
{
    [Serializable]
    public class DeckComponentDamageDealer : DeckComponent
    {
        private DeckDataDamage _dataDamage;

        public Action<int> OnDamageDealRequested;

        private bool _canAttack = true;

        protected override void InternalPreInitialize()
        {
            _dataDamage = agent.GetAgentData<DeckDataDamage>();
        }

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

            //TODO: Handle equipments
            
            // if (!agent.TryGetDeckComponent<DeckComponentEquipmentManager>(out var equipmentManager))
            // {
                // return;
            // }
            // Debug.LogError("5");
            //
            // if (!agent.TryGetComponent<DeckComponentInventory>(out var inventoryComponent))
            // {
            //     return;
            // }
            // Debug.LogError("6");
            //
            // if (!inventoryComponent.TryGetItemWithTag(healthComponent.GetDamagingTags(), out var requiredItem))
            // {
            //     DeckEventNotificationRequested.Create("Can't damage this " + target.name).Send();
            //     return;
            // }
            // Debug.LogError("7");

            // equipmentManager.SetItemToHold(requiredItem);
            // Debug.LogError("8");

            var to = target.GetDeckComponent<DeckComponentHealth>();
            if (to.IsDead)
            {
                return;
            }

            OnAttackStart();
            OnAttack();

            to.ReduceHealth(GetAgent(), GetFinalDamageValue(), true);
        }

        public void OnAttackStart()
        {
            OnDamageDealRequested?.Invoke(_dataDamage.GetAttackDuration());
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