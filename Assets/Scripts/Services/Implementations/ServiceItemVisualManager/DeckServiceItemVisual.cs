using Deck.Item;
using Deck.Services;
using Deck.UI.InGame;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    public class DeckServiceItemVisual : DeckServiceBase
    {
        private DeckItemVisualProviderBasic[] _itemVisualProviders;
        private DiContainer _container;

        [Inject]
        private void Inject(DeckItemVisualProviderBasic[] itemVisualProviders, DiContainer container)
        {
            _itemVisualProviders = itemVisualProviders;
            _container = container;
            
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                deckItemVisualProvider.Initialize();
            }
        }

        public DeckItemVisual RentItemVisual(DeckId deckId)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.RentIfHasItemVisual(deckId, out var itemVisual))
                {
                    return itemVisual;
                }
            }

            DeckLogger.Error("No item visual found for id " + deckId.ID);
            return null;
        }

        public void ReturnItemVisual(DeckItemVisual itemVisual)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.ReturnIfHasItemVisual(itemVisual))
                {
                    return;
                }
            }

            DeckLogger.Error("No item visual found for id " + itemVisual.PrefabId);
        }
    }
}