using System.Collections.Generic;
using Deck.Data.Item;
using Deck.Item;
using Deck.Utility.Logger;
using UnityEngine;
using Utility.MonoBehaviours;

namespace Deck.Components
{
    public class DeckComponentItemHolder : DeckComponent, IDeckItemItemHolder
    {
        private Dictionary<string, Transform> _bindedTransforms;
        private DeckItemVisual _currentlyEquippedItem;
        private string _currentlyEquippedItemState;

        protected override void InternalPostInitialize()
        {
            _bindedTransforms = new Dictionary<string, Transform>();
            var binders = holder.GetComponentsInChildren<DeckTransformBinder>();
            if (binders.Length == 0)
            {
                DeckLogger.Inform("Item holder doesn't have any transform bindings");
            }

            foreach (var binder in binders)
            {
                if (_bindedTransforms.ContainsKey(binder.GetKey()))
                {
                    DeckLogger.Inform("More than one item with same key first one will be accepted");
                    continue;
                }

                _bindedTransforms[binder.GetKey()] = binder.transform;
            }
        }

        public void SetItemToHold(DeckDataItem itemToHold)
        {
            if (_currentlyEquippedItem != null)
            {
                Object.Destroy(_currentlyEquippedItem.gameObject);
                holder.GetDeckComponent<IDeckAnimationSetBool>().Animate(_currentlyEquippedItemState, false);
            }

            if (!itemToHold.Holdable)
            {
                return;
            }

            var animationName = itemToHold.AnimationName;

            if (_bindedTransforms.TryGetValue(animationName, out var target))
            {
                EquipItem(itemToHold, target);
                SetAnimation(itemToHold);
            }
            else
            {
                DeckLogger.Inform("No target match for the requested item can't equip");
            }
        }

        private void SetAnimation(DeckDataItem itemToHold)
        {
            holder.GetDeckComponent<IDeckAnimationSetBool>().Animate(_currentlyEquippedItemState, true);
            _currentlyEquippedItemState = itemToHold.AnimationName;
        }

        private void EquipItem(DeckDataItem itemToHold, Transform target)
        {
            _currentlyEquippedItem = Object.Instantiate(itemToHold.Representation);
            _currentlyEquippedItem.transform.SetParent(target, true);
            _currentlyEquippedItem.transform.localPosition = itemToHold.Representation.LocalEquipPosition;
            _currentlyEquippedItem.transform.localRotation = itemToHold.Representation.LocalEquipRotation;
            _currentlyEquippedItem.OnEquip();
        }
    }
}