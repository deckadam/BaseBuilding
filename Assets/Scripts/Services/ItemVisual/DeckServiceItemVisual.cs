using System.Collections.Generic;
using Base;
using ItemVisualProviders;
using UnityEngine;
using Utility;
using Zenject;

namespace Services.ItemVisual
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

        public override void DeInitialize()
        {
            foreach (var deckItemVisualProvider in _itemVisualProviders)
            {
                deckItemVisualProvider.OnDeInitialize();
            }
        }

        public bool RequestItemVisual(DeckAgent agent, DeckId deckId, out DeckItemVisual result, Vector2Int cellIndex = default)
        {
            if (_itemVisualProviderDictionary.TryGetValue(deckId.ID, out var itemVisualProvider))
            {
                if (itemVisualProvider.RequestItemVisual(agent, deckId, cellIndex, out result))
                {
                    return true;
                }
            }

            DeckLogger.Error("Item visual not found");
            result = null;
            return false;
        }

        public bool RequestMultipleItemVisual(DeckAgent[] agents, DeckId deckId, int count, Vector2Int[] indices, out DeckItemVisual[] result)
        {
            if (_itemVisualProviderDictionary.TryGetValue(deckId.ID, out var itemVisualProvider))
            {
                result = new DeckItemVisual[count];
                if (itemVisualProvider.RequestBulkItemVisuals(agents, deckId, indices, out result)) return true;

                DeckLogger.Error($"Multiple item visual couldn't be rented");
                result = null;
                return false;
            }
            else
            {
                DeckLogger.Error($"Item visual not found {deckId.ID}");
                result = null;
                return false;
            }
        }

        public bool RequestMultipleItemVisual(DeckAgent[] agents, DeckId deckId, int count, out DeckItemVisual[] result)
        {
            if (_itemVisualProviderDictionary.TryGetValue(deckId.ID, out var itemVisualProvider))
            {
                result = new DeckItemVisual[count];
                if (itemVisualProvider.RequestBulkItemVisuals(agents, deckId, out result)) return true;

                DeckLogger.Error($"Multiple item visual couldn't be rented");
                result = null;
                return false;
            }
            else
            {
                DeckLogger.Error($"Item visual not found {deckId.ID}");
                result = null;
                return false;
            }
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

        public void ReturnItemVisuals(DeckItemVisual[] itemVisuals)
        {
            foreach (var itemVisual in itemVisuals)
            {
                if (_itemVisualProviderDictionary.TryGetValue(itemVisual.PrefabId.ID, out var itemVisualProvider))
                {
                    itemVisualProvider.ReturnItemVisual(itemVisual);
                }
                else
                {
                    DeckLogger.Error("No item visual found for id " + itemVisual.name);
                    return;
                }
            }
        }
    }
}