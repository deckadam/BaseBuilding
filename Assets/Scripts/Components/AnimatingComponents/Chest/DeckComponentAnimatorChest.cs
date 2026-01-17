using System;
using System.Linq;
using Base;
using Components.AnimatingComponents.Interfaces;
using Components.Inventory;
using Utility;
using Utility.Animators;
using Utility.Constants;

namespace Components.AnimatingComponents.Chest
{
    public class DeckComponentAnimatorChest : DeckComponent, IDeckAnimationImmediatePlay
    {
        private DeckRotationAnimationSwitcher _rotationAnimationSwitcher;
        private DeckComponentInventory _componentInventory;

        protected override void InternalPreInitialize()
        {
            agent.OnItemVisualChanged += OnItemVisualChanged;
        }

        private void OnItemVisualChanged()
        {
            _componentInventory = agent.GetDeckComponent<DeckComponentInventory>();
            if (_componentInventory == null)
            {
                throw new Exception("Inventory component not found");
            }

            _componentInventory.OnInventoryViewingChanged += Animate;

            _rotationAnimationSwitcher = agent.GetComponentsInChildren<DeckRotationAnimationSwitcher>().FirstOrDefault(item => item.GetTag() == DeckConstantsAgents.DECK_CHEST_ANIMATOR);
        }

        public override void DeInitialize()
        {
            _componentInventory.OnInventoryViewingChanged += Animate;
        }

        private void Animate(bool state)
        {
            _rotationAnimationSwitcher.Animate(state);
        }

        public void Animate(string name)
        {
            DeckLogger.Inform(agent.PrefabId + " can not be animated with with this method");
        }
    }
}