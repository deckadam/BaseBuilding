using System;
using System.Linq;
using Unility.Animators;
using Deck.Utility.Constants;

namespace Deck.Components
{
    public class DeckComponentAnimatorChest : DeckComponent
    {
        private DeckRotationAnimationSwitcher _rotationAnimationSwitcher;
        private DeckComponentInventory _componentInventory;

        protected override void Initialize()
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
    }
}