using System;
using System.Collections.Generic;
using Deck.Data.Item;
using Deck.Item;
using Deck.ItemVisualProviders;
using Deck.Save;
using Deck.UI.InGame;
using Deck.Utility.Logger;
using UnityEngine;
using Deck.Utility.MonoBehaviours;
using Zenject;

namespace Deck.Commands
{
    public class DeckComponentEquipmentManager : DeckComponent, IDeckItemItemHolder
    {
        private Dictionary<string, Transform> _bindedTransforms;
        private DeckItemVisual _currentlyEquippedItem;
        private DeckDataItem _currentlyEquippedItemData;
        private string _currentlyEquippedItemState;
        private DeckBinderItem _binderItem;

        [Inject]
        private void Inject(DeckBinderItem binderItem)
        {
            _binderItem = binderItem;
        }

        protected override void InternalPostInitialize()
        {
            _bindedTransforms = new Dictionary<string, Transform>();
            var binders = agent.GetComponentsInChildren<DeckTransformBinder>();
            if (binders.Length == 0)
            {
                DeckLogger.Inform("Item holder doesn't have any transform bindings");
            }

            ProcessBinders(binders);
        }

        private void ProcessBinders(DeckTransformBinder[] binders)
        {
            foreach (var binder in binders)
            {
                ProcessKeys(binder);
            }
        }

        private void ProcessKeys(DeckTransformBinder binder)
        {
            foreach (var temp in binder.GetKeys())
            {
                AddEquipmentPosition(temp, binder);
            }
        }

        private void AddEquipmentPosition(string temp, DeckTransformBinder binder)
        {
            if (!_bindedTransforms.ContainsKey(temp))
            {
                _bindedTransforms[temp] = binder.transform;
                return;
            }

            DeckLogger.Inform("More than one item with same key first one will be accepted");
        }

        public void SetItemToHold(DeckDataItem itemToHold)
        {
            if (_currentlyEquippedItem != null && _currentlyEquippedItem != itemToHold.Representation)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(_currentlyEquippedItem);
                _currentlyEquippedItem = null;
                agent.GetDeckComponent<IDeckAnimationSetBool>().Animate(_currentlyEquippedItemState, false);
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
            _currentlyEquippedItemState = itemToHold.AnimationName;
        }

        private void EquipItem(DeckDataItem itemData, Transform target)
        {
            _currentlyEquippedItemData = itemData;
            _currentlyEquippedItem = Deck.GetService<DeckServiceItemVisual>().RentItemVisual(itemData.Representation.PrefabId);
            _currentlyEquippedItem.transform.SetParent(target, true);
            _currentlyEquippedItem.transform.localPosition = _currentlyEquippedItem.LocalEquipPosition;
            _currentlyEquippedItem.transform.eulerAngles = _currentlyEquippedItem.LocalEquipRotation;
            _currentlyEquippedItem.OnEquip();
        }

        public DeckItemVisual GetEquippedItem() => _currentlyEquippedItem;

        public override object GetData()
        {
            if (_currentlyEquippedItem != null)
            {
                var data = new SaveData
                {
                    equippedItemName = _currentlyEquippedItemData.Name
                };

                return data;
            }

            return null;
        }

        public override void LoadData(string value)
        {
            var data = DeckSaveUtility.GetDeserializedData<SaveData>(value);
            var itemData = _binderItem.GetItemWithName(data.equippedItemName);
            SetItemToHold(itemData);
        }

        [Serializable]
        private struct SaveData
        {
            public string equippedItemName;
        }
    }
}