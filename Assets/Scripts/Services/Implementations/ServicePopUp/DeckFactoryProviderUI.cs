using System;
using System.Collections.Generic;
using Deck.UI.Inventory;
using Deck.UI.Item;
using UnityEngine;
using Zenject;

namespace Deck.Events
{
    public class DeckFactoryProviderUI
    {
        [Inject] private DeckInventoryPopUp.Factory inventoryPopUpFactory;
        [Inject] private DeckConfirmationPopUp.Factory confirmationPopUpFactory;
        [Inject] private DeckUIItemDisplayer.Factory itemDisplayerFactory;

        private Dictionary<Type, object> _factories;

        [Inject]
        public void Initialize()
        {
            _factories = new Dictionary<Type, object>();
            _factories[typeof(DeckInventoryPopUp)] = inventoryPopUpFactory;
            _factories[typeof(DeckConfirmationPopUp)] = confirmationPopUpFactory;
            _factories[typeof(DeckUIItemDisplayer)] = itemDisplayerFactory;
        }

        public J GetFactory<T, J>()
        {
            return (J)_factories[typeof(T)];
        }
    }
}