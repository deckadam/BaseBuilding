using System;
using System.Collections.Generic;
using Deck.UI.Inventory;
using Zenject;

namespace Deck.Services.Implementations
{
    public class DeckPopUpFactoryProvider
    {
        [Inject] private DeckInventoryPopUp.Factory inventoryPopUpFactory;

        private Dictionary<Type, object> _factories;

        public void Initialize()
        {
            _factories = new Dictionary<Type, object>();
            _factories[typeof(DeckInventoryPopUp)] = inventoryPopUpFactory;
        }

        public J GetFactory<T, J>()
        {
            return (J) _factories[typeof(T)];
        }
    }
}