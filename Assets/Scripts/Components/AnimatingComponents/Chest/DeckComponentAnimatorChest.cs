using System;
using System.Linq;
using Deck.Components;
using Deck.Utility;
using Deck.Animators;
using Deck.Utility.Logger;

namespace Deck.Components
{
    public class DeckComponentAnimatorChest : DeckComponent, IDeckAnimationImmediatePlay
    {
        private DeckRotationAnimationSwitcher _rotationAnimationSwitcher;
        private DeckComponentInventory _componentInventory;

        protected override void InternalPreInitialize()
        {
            _componentInventory = holder.GetDeckComponent<DeckComponentInventory>();
            if (_componentInventory == null)
            {
                throw new Exception("Inventory component not found");
            }

            _componentInventory.OnInventoryViewingChanged += Animate;

            _rotationAnimationSwitcher = holder.GetComponentsInChildren<DeckRotationAnimationSwitcher>().FirstOrDefault(item => item.GetTag() == DeckConstantsAgents.DECK_CHEST_ANIMATOR);
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
            DeckLogger.Inform(holder.GetId() + " can not be animated with with this method");
        }
    }
}