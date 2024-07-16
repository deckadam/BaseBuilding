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

        [Inject]
        private void Inject(DeckItemVisualProviderBasic[] itemVisualProviders)
        {
            _itemVisualProviders = itemVisualProviders;

            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                deckItemVisualProvider.Initialize();
            }
        }

        public bool RequestItemVisual(DeckId deckId, out DeckItemVisual itemVisual, Vector2Int cellIndex = default, bool isInternal = true)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.RequestItemVisual(deckId, cellIndex, out itemVisual,isInternal: isInternal))
                {
                    return true;
                }
            }

            itemVisual = null;
            return false;
        }

        public void ReturnItemVisual(DeckItemVisual itemVisual)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.ReturnItemVisual(itemVisual))
                {
                    return;
                }
            }

            DeckLogger.Error("No item visual found for id " + itemVisual.PrefabId);
        }
    }
}