using System;
using System.Collections.Generic;
using Deck.Item;
using Deck.Save;
using Deck.Save.Data;
using Deck.UI.InGame;
using Deck.Utility.Class;
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

        private Dictionary<int, Stack<DeckItemVisual>> _activeItemVisuals;

        private Transform _poolParent;
        private DeckInstanceCreator _instanceCreator;

        [Inject]
        private void Inject(DeckInstanceCreator instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public void Initialize()
        {
            _activeItemVisuals = new Dictionary<int, Stack<DeckItemVisual>>();
            _poolParent = new GameObject().transform;
            _poolParent.name = name + "Pool";
            foreach (var visualSet in itemVisualSets)
            {
                _activeItemVisuals.Add(visualSet.PrefabId.ID, new Stack<DeckItemVisual>());
            }

            OnInitialize();
        }

        public virtual bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool alert = true)
        {
            return RentIfHasItemVisual(prefabId, out itemVisual, alert);
        }

        public virtual bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            return ReturnIfHasItemVisual(itemVisual);
        }

        protected bool RentIfHasItemVisual(DeckId prefabId, out DeckItemVisual itemVisual, bool isInternal = true)
        {
            itemVisual = null;
            foreach (var visualSet in itemVisualSets)
            {
                if (Equals(visualSet.PrefabId, prefabId))
                {
                    if (!TryGetItemVisual(visualSet, out itemVisual))
                        continue;

                    itemVisual.gameObject.SetActive(true);
                    Deck.GetService<DeckServiceFinder>().RegisterItemVisual(itemVisual);
                    if (isInternal)
                    {
                        OnSpawned(itemVisual);
                    }

                    return true;
                }
            }

            itemVisual = default;
            return false;
        }

        protected bool ReturnIfHasItemVisual(DeckItemVisual itemVisual)
        {
            if (_activeItemVisuals.TryGetValue(itemVisual.PrefabId.ID, out var list))
            {
                list.Push(itemVisual);
                itemVisual.gameObject.SetActive(false);
                Deck.GetService<DeckServiceFinder>().RemoveItemVisual(itemVisual);
                OnDespawned(itemVisual);

                return true;
            }

            return false;
        }

        protected virtual bool TryGetItemVisual(DeckItemVisual visual, out DeckItemVisual result)
        {
            if (_activeItemVisuals.TryGetValue(visual.PrefabId.ID, out var list))
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

        public string GetSaveData()
        {
            return OnSaveDataRequested();
        }

        public void LoadData(string value)
        {
            OnLoadDataRequested(value);
        }

        protected virtual string OnSaveDataRequested()
        {
            var saveDataHolder = new SaveDataHolder
            {
                saveData = new List<SaveData>()
            };
            var saveData = saveDataHolder.saveData;
            foreach (var activeItemVisual in _activeItemVisuals)
            {
                var clone = activeItemVisual.Value.Clone();
                foreach (var deckItemVisual in clone)
                {
                    saveData.Add(new SaveData
                    {
                        prefabId = deckItemVisual.PrefabId.ID.ToString(),
                        uniqueId = deckItemVisual.UniqueId.ID.ToString(),
                        position = deckItemVisual.transform.position,
                        rotation = deckItemVisual.transform.eulerAngles,
                        scale = deckItemVisual.transform.localScale
                    });
                }
            }

            return DeckSaveUtility.GetSerializedData(saveDataHolder);
        }

        protected virtual void OnLoadDataRequested(string value)
        {
            var saveDataHolder = DeckSaveUtility.GetDeserializedData<SaveDataHolder>(value);

            foreach (var saveData in saveDataHolder.saveData)
            {
                var id = new DeckId(saveData.prefabId);
                RentIfHasItemVisual(id, out var itemVisual, false);
                itemVisual.SetUniqueId(id);
                itemVisual.transform.position = saveData.position;
                itemVisual.transform.eulerAngles = saveData.rotation;
                itemVisual.transform.localScale = saveData.scale;
                _activeItemVisuals[itemVisual.PrefabId.ID].Push(itemVisual);
            }
        }

        [Serializable]
        private struct SaveDataHolder
        {
            public List<SaveData> saveData;
        }

        [Serializable]
        private struct SaveData
        {
            public string prefabId;
            public string uniqueId;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
        }
    }
}