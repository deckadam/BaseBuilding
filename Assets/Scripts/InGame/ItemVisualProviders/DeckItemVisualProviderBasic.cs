using System;
using System.Collections.Generic;
using Deck.Item;
using Deck.UI.InGame;
using Services.AgentFinder;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderBasic", menuName = "Service/ItemVisualManager/DeckItemVisualProviderBasic")]
    public class DeckItemVisualProviderBasic : ScriptableObject
    {
        [SerializeField] private DeckItemVisual[] itemVisualSets;

        private Dictionary<Guid, List<DeckItemVisual>> activeItemVisuals;

        private DiContainer _container;

        private Transform _poolParent;

        public void Initialize(DiContainer container)
        {
            _container = container;
            activeItemVisuals = new Dictionary<Guid, List<DeckItemVisual>>();
            _poolParent = new GameObject().transform;
            _poolParent.name = name + "Pool";
            foreach (var visualSet in itemVisualSets)
            {
                activeItemVisuals.Add(visualSet.PrefabId.ID, new List<DeckItemVisual>());
            }

            OnInitialize();
        }

        public bool RentIfHasItemVisual(DeckId id, out DeckItemVisual itemVisual)
        {
            itemVisual = null;
            foreach (var visualSet in itemVisualSets)
            {
                if (visualSet.PrefabId == id)
                {
                    if (!TryGetItemVisual(visualSet, out itemVisual))
                        continue;

                    itemVisual.gameObject.SetActive(true);
                    Deck.GetService<DeckServiceFinder>().RegisterItemVisual(itemVisual);
                    OnSpawned(itemVisual);
                    return true;
                }
            }

            itemVisual = default;
            return false;
        }

        public bool ReturnIfHasItemVisual(DeckItemVisual itemVisual)
        {
            foreach (var visualSet in itemVisualSets)
            {
                if (visualSet.PrefabId == itemVisual.PrefabId)
                {
                    activeItemVisuals[visualSet.PrefabId.ID].Add(itemVisual);
                    itemVisual.gameObject.SetActive(false);
                    Deck.GetService<DeckServiceFinder>().RemoveItemVisual(itemVisual);
                    OnDespawned(itemVisual);
                    return true;
                }
            }

            return false;
        }

        private bool TryGetItemVisual(DeckItemVisual visual, out DeckItemVisual result)
        {
            if (activeItemVisuals.TryGetValue(visual.PrefabId.ID, out var itemVisuals))
            {
                if (itemVisuals.Count > 0)
                {
                    result = itemVisuals[^1];
                    itemVisuals.RemoveAt(itemVisuals.Count - 1);

                    return true;
                }

                result = CreateItemVisual(visual);
                return true;
            }

            result = null;
            return false;
        }

        private DeckItemVisual CreateItemVisual(Object visual)
        {
            var newObject = _container.InstantiatePrefab(visual);
            newObject.transform.SetParent(_poolParent);
            var temp = newObject.GetComponent<DeckItemVisual>();
            temp.SetNewUniqueId();
            return temp;
        }

        protected virtual void OnSpawned(DeckItemVisual itemVisual)
        {
        }

        protected virtual void OnDespawned(DeckItemVisual itemVisual)
        {
        }

        protected virtual void OnInitialize()
        {
        }
    }
}