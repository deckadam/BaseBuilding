using Deck.Base.Id;
using Deck.Components;
using Deck.Services;
using Deck.Utility;
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

        public bool RequestItemVisual(DeckAgent agent,DeckId deckId, out DeckItemVisual itemVisual, Vector2Int cellIndex = default)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.RequestItemVisual(agent,deckId, cellIndex, out itemVisual))
                {
                    return true;
                }
            }

            Debug.LogError("RequestItemVisual failed");
            itemVisual = null;
            return false;
        }

        public bool RequestItemVisual(DeckId deckId, out DeckItemVisual itemVisual)
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                if (deckItemVisualProvider.RequestItemVisual(deckId, out itemVisual))
                {
                    return true;
                }
            }

            Debug.LogError("RequestItemVisual failed");
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

            DeckLogger.Error("No item visual found for id " + itemVisual.name);
        }
    }
}