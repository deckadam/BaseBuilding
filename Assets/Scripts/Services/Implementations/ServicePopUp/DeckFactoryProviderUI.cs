using System;
using System.Collections.Generic;
using Deck.UI.Inventory;
using Deck.UI.Item;
using Deck.UI.Stats;
using Zenject;

namespace Deck.Services
{
    public class DeckFactoryProviderUI
    {
        [Inject] private DeckInventoryPopUp.Factory inventoryPopUpFactory;
        [Inject] private DeckConfirmationPopUp.Factory confirmationPopUpFactory;
        [Inject] private DeckStatsPopUp.Factory statsPopUpFactory;
        [Inject] private DeckUIItemDisplayer.Factory itemDisplayerFactory;
        [Inject] private DeckUIStatContainer.Factory statContainerFactory;

        private Dictionary<Type, object> _factories;

        [Inject]
        public void Initialize()
        {
            _factories = new Dictionary<Type, object>();
            _factories[typeof(DeckInventoryPopUp)] = inventoryPopUpFactory;
            _factories[typeof(DeckConfirmationPopUp)] = confirmationPopUpFactory;
            _factories[typeof(DeckUIItemDisplayer)] = itemDisplayerFactory;
            _factories[typeof(DeckStatsPopUp)] = statsPopUpFactory;
            _factories[typeof(DeckUIStatContainer)] = statContainerFactory;
        }

        public J GetFactory<T, J>()
        {
            return (J)_factories[typeof(T)];
        }
    }
}