using System.Collections.Generic;
using Deck.Base.Id;
using Deck.Components;
using Deck.Services;
using Deck.Utility;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    public class DeckServiceItemVisual : DeckServiceBase
    {
        private DeckItemVisualProviderBasic[] _itemVisualProviders;

        private Dictionary<int, DeckItemVisualProviderBasic> _itemVisualProviderDictionary;

        [Inject]
        private void Inject(DeckItemVisualProviderBasic[] itemVisualProviders)
        {
            _itemVisualProviders = itemVisualProviders;
        }

        public override void Initialize()
        {
            _itemVisualProviderDictionary = new Dictionary<int, DeckItemVisualProviderBasic>();

            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                deckItemVisualProvider.Initialize();
                var supportedItemVisuals = deckItemVisualProvider.GetSupportedItemVisuals();
                foreach (var supportedItemVisual in supportedItemVisuals)
                {
                    _itemVisualProviderDictionary[supportedItemVisual] = deckItemVisualProvider;
                }
            }
        }

        public bool RequestItemVisual(DeckAgent agent, DeckId deckId, out DeckItemVisual itemVisual, Vector2Int cellIndex = default)
        {
            if (_itemVisualProviderDictionary.TryGetValue(deckId.ID, out var itemVisualProvider))
            {
                if (itemVisualProvider.RequestItemVisual(agent, deckId, cellIndex, out itemVisual))
                {
                    return true;
                }
            }

            itemVisual = null;
            return false;
        }

        public bool RequestItemVisual(DeckId deckId, out DeckItemVisual itemVisual)
        {
            if (_itemVisualProviderDictionary.TryGetValue(deckId.ID, out var itemVisualProvider))
            {
                if (itemVisualProvider.RequestItemVisual(deckId, out itemVisual))
                {
                    return true;
                }
            }

            itemVisual = null;
            return false;
        }

        public void ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (_itemVisualProviderDictionary.TryGetValue(itemVisual.PrefabId.ID, out var itemVisualProvider))
            {
                itemVisualProvider.ReturnItemVisual(itemVisual);
                return;
            }

            DeckLogger.Error("No item visual found for id " + itemVisual.name);
        }
    }
}