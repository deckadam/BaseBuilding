using System.Collections.Generic;
using Deck.Item;
using Deck.Save.Data;
using Deck.UI.InGame;
using Services.AgentFinder;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderBasic", menuName = "Service/ItemVisualManager/DeckItemVisualProviderBasic")]
    public class DeckItemVisualProviderBasic : ScriptableObject
    {
        [SerializeField, InlineEditor] private DeckItemVisual[] itemVisualSets;

        private Dictionary<int, Stack<DeckItemVisual>> activeItemVisuals;

        private Transform _poolParent;
        private DeckInstanceCreator _instanceCreator;

        [Inject]
        private void Inject(DeckInstanceCreator instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public void Initialize()
        {
            activeItemVisuals = new Dictionary<int, Stack<DeckItemVisual>>();
            _poolParent = new GameObject().transform;
            _poolParent.name = name + "Pool";
            foreach (var visualSet in itemVisualSets)
            {
                activeItemVisuals.Add(visualSet.PrefabId.ID, new Stack<DeckItemVisual>());
            }

            OnInitialize();
        }

        public bool RentIfHasItemVisual(DeckId id, out DeckItemVisual itemVisual)
        {
            itemVisual = null;
            foreach (var visualSet in itemVisualSets)
            {
                if (Equals(visualSet.PrefabId, id))
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
            if (activeItemVisuals.TryGetValue(itemVisual.PrefabId.ID, out var list))
            {
                list.Push(itemVisual);
                itemVisual.gameObject.SetActive(false);
                Deck.GetService<DeckServiceFinder>().RemoveItemVisual(itemVisual);
                OnDespawned(itemVisual);

                return true;
            }

            return false;
        }

        private bool TryGetItemVisual(DeckItemVisual visual, out DeckItemVisual result)
        {
            if (activeItemVisuals.TryGetValue(visual.PrefabId.ID, out var list))
            {
                if (list.Count > 0)
                {
                    result = list.Pop();
                    return true;
                }

                result = CreateItemVisual(visual.PrefabId.ID);
                return true;
            }

            result = null;
            return false;
        }

        private DeckItemVisual CreateItemVisual(int id)
        {
            var newObject = _instanceCreator.CreateNewItemVisualInstance(id);
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