using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Deck.Base;
using Instancing;
using Services;
using Services.Finder;
using UnityEngine;
using Utility;
using Zenject;

namespace ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderBase", menuName = "Service/ItemVisualManager/DeckItemVisualProviderBase")]
    public class DeckItemVisualProviderBasic : ScriptableObject
    {
        [SerializeField] protected List<DeckItemVisual> itemVisualSets;

        private HashSet<int> _supportedItemVisuals;
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public void AddItemVisual(DeckItemVisual itemVisual)
        {
            if (!itemVisualSets.Contains(itemVisual))
            {
                itemVisualSets.Add(itemVisual);
            }
            else
            {
                DeckLogger.Error("Already contains item visual");
            }

            itemVisualSets = itemVisualSets.Where(x => x != null).ToList();
        }

        public void Initialize()
        {
            _supportedItemVisuals = new HashSet<int>();
            foreach (var deckItemVisual in itemVisualSets)
            {
                _supportedItemVisuals.Add(deckItemVisual.PrefabId.ID);
            }

            InternalOnInitialize();
        }

        protected virtual void InternalOnInitialize()
        {
        }

        public void OnDeInitialize()
        {
            InternalOnDeInitialize();
        }

        protected virtual void InternalOnDeInitialize()
        {
        }

        public virtual bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            return RentIfHasItemVisual(prefabId, out itemVisual);
        }

        public virtual bool RequestItemVisual(DeckId prefabId, out DeckItemVisual itemVisual)
        {
            return RentIfHasItemVisual(prefabId, out itemVisual);
        }

        public virtual bool RequestMultipleItemVisuals(DeckAgent agent,DeckId prefabId, Vector2Int[] indices,out DeckItemVisual[] itemVisuals)
        {
            throw new Exception("Multiple rent is not implemented for basic provider");
        }

        public virtual bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            return ReturnIfHasItemVisual(itemVisual);
        }

        protected bool RentIfHasItemVisual(DeckId prefabId, out DeckItemVisual itemVisual)
        {
            if (_supportedItemVisuals.Contains(prefabId.ID))
            {
                itemVisual = _instanceProvider.RentItemVisual(prefabId);
                itemVisual.gameObject.SetActive(true);
                DeckServiceProvider.GetService<DeckServiceFinder>().RegisterItemVisual(itemVisual);
                return true;
            }

            itemVisual = default;
            return false;
        }

        protected bool ReturnIfHasItemVisual(DeckItemVisual itemVisual)
        {
            if (!_supportedItemVisuals.Contains(itemVisual.PrefabId.ID)) return false;

            _instanceProvider.ReturnItemVisual(itemVisual);
            DeckServiceProvider.GetService<DeckServiceFinder>().RemoveItemVisual(itemVisual);

            return true;
        }

        public HashSet<int> GetSupportedItemVisuals()
        {
            return _supportedItemVisuals;
        }

        protected bool IsSupportedItemVisual(DeckId prefabId)
        {
            return _supportedItemVisuals.Contains(prefabId.ID);
        }

        protected bool IsSupportedItemVisual(DeckItemVisual itemVisual)
        {
            return _supportedItemVisuals.Contains(itemVisual.PrefabId.ID);
        }
    }
}