using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Utility;
using Zenject;

namespace Instancing
{
    [CreateAssetMenu(menuName = "Deck/Data/Resolver/Instance creator", fileName = "Instance creator")]
    public class DeckInstanceProvider : ScriptableObject
    {
        [SerializeField] private List<DeckAgent> agents;
        [SerializeField] private List<DeckItemVisual> itemVisuals;
        [SerializeField] private List<DeckUIElement> uiElements;

        private Dictionary<int, DeckAgent> _agentDictionary;
        private Dictionary<int, DeckItemVisual> _itemVisualDictionary;
        private Dictionary<int, DeckUIElement> _uiElementDictionary;

        private Dictionary<int, Stack<DeckAgent>> _agentPool;
        private Dictionary<int, Stack<DeckItemVisual>> _itemVisualPool;
        private Dictionary<int, Stack<DeckUIElement>> _uiElementPool;

        private Dictionary<Type, DeckId> _agentByType;
        private Dictionary<Type, DeckId> _itemVisualByType;
        private Dictionary<Type, DeckId> _uiElementByType;

        private DiContainer _container;

        private Transform _agentContainer;
        private Transform _itemVisualContainer;
        private Transform _uiContainer;

#if UNITY_EDITOR
        [DidReloadScripts]
        [Button]
        private static void OnScriptsReloaded()
        {
            var instanceCreator = Resources.Load<DeckInstanceProvider>("Data/Deck Instance Provider");
            instanceCreator.itemVisuals = new List<DeckItemVisual>();

            instanceCreator.agents = new List<DeckAgent>();
            EditorInitialize();
        }

        [Button]
        private void OnValidate()
        {
            EditorInitialize();
        }

        public void AddItemVisual(DeckItemVisual visual)
        {
            if (!itemVisuals.Contains(visual))
            {
                itemVisuals.Add(visual);
            }
            else
            {
                DeckLogger.Error("Duplicate deck item visual");
            }

            itemVisuals.RemoveAll(item => item == null);
        }

        public void AddAgent(DeckAgent agent)
        {
            if (!agents.Contains(agent))
            {
                agents.Add(agent);
            }
            else
            {
                DeckLogger.Error("Duplicate deck item visual");
            }

            agents.RemoveAll(item => item == null);
        }

        [MenuItem("Deck/Collect Instances")]
        private static void EditorInitialize()
        {
            var instanceProvider = Resources.FindObjectsOfTypeAll<DeckInstanceProvider>()[0];
            instanceProvider.agents = new List<DeckAgent>();
            var agents = Resources.FindObjectsOfTypeAll(typeof(DeckAgent));
            var distinctAgents = agents.Select(item => item).Distinct();
            foreach (var agent in distinctAgents)
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(agent))
                {
                    continue;
                }

                var instance = (DeckAgent)agent;

                if (!string.IsNullOrEmpty(instance.gameObject.scene.name))
                {
                    continue;
                }

                if (!instanceProvider.agents.Any(item => item.PrefabId.Equals(instance.PrefabId)))
                {
                    instanceProvider.agents.Add(instance);
                }
            }

            instanceProvider.itemVisuals = new List<DeckItemVisual>();
            var itemVisuals = Resources.FindObjectsOfTypeAll(typeof(DeckItemVisual));
            var distinctItemVisuals = itemVisuals.Select(item => item).Distinct();
            foreach (var itemVisual in distinctItemVisuals)
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(itemVisual))
                {
                    continue;
                }

                var instance = (DeckItemVisual)itemVisual;

                if (instance != null && !string.IsNullOrEmpty(instance.gameObject.scene.name))
                {
                    continue;
                }

                if (!instanceProvider.itemVisuals.Any(item => item.PrefabId.Equals(instance.PrefabId)))
                {
                    instanceProvider.itemVisuals.Add(instance);
                }
            }

            instanceProvider.uiElements = new List<DeckUIElement>();
            var uiElements = Resources.FindObjectsOfTypeAll(typeof(DeckUIElement));
            var distinctUiElements = uiElements.Select(item => item).Distinct();
            foreach (var uiElement in distinctUiElements)
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(uiElement))
                {
                    continue;
                }

                var instance = (DeckUIElement)uiElement;

                if (!string.IsNullOrEmpty(instance.gameObject.scene.name))
                {
                    continue;
                }

                if (!instanceProvider.uiElements.Any(item => item.PrefabId.Equals(instance.PrefabId)))
                {
                    instanceProvider.uiElements.Add(instance);
                }
            }

            instanceProvider.agents.RemoveAll(item => item == null);
            instanceProvider.itemVisuals.RemoveAll(item => item == null);
            instanceProvider.uiElements.RemoveAll(item => item == null);
        }
#endif

        [Inject]
        private void Inject(DiContainer container)
        {
            _container = container;
            Initialize();
        }

        private void Initialize()
        {
            _agentDictionary = new Dictionary<int, DeckAgent>();
            _agentPool = new Dictionary<int, Stack<DeckAgent>>();
            _agentByType = new Dictionary<Type, DeckId>();
            foreach (var agent in agents)
            {
                _agentDictionary[agent.PrefabId.Id] = agent;
                _agentPool[agent.PrefabId.Id] = new Stack<DeckAgent>();
                _agentByType[agent.GetType()] = agent.PrefabId;
            }

            _itemVisualDictionary = new Dictionary<int, DeckItemVisual>();
            _itemVisualPool = new Dictionary<int, Stack<DeckItemVisual>>();
            _itemVisualByType = new Dictionary<Type, DeckId>();
            foreach (var itemVisual in itemVisuals)
            {
                _itemVisualDictionary[itemVisual.PrefabId.Id] = itemVisual;
                _itemVisualPool[itemVisual.PrefabId.Id] = new Stack<DeckItemVisual>();
                _itemVisualByType[itemVisual.GetType()] = itemVisual.PrefabId;
            }

            _uiElementDictionary = new Dictionary<int, DeckUIElement>();
            _uiElementPool = new Dictionary<int, Stack<DeckUIElement>>();
            _uiElementByType = new Dictionary<Type, DeckId>();
            foreach (var uiElement in uiElements)
            {
                _uiElementDictionary[uiElement.PrefabId.Id] = uiElement;
                _uiElementPool[uiElement.PrefabId.Id] = new Stack<DeckUIElement>();
                _uiElementByType[uiElement.GetType()] = uiElement.PrefabId;
            }

            _agentContainer = new GameObject()
            {
                name = "Agent Container"
            }.transform;

            _itemVisualContainer = new GameObject()
            {
                name = "Item Visual Container"
            }.transform;

            _uiContainer = new GameObject()
            {
                name = "UI Container"
            }.transform;
        }

        public DeckAgent RentAgent(DeckId id, int uniqueId = 0)
        {
            return RentAgent(id.Id, uniqueId);
        }

        public DeckAgent RentAgent<T>(int uniqueId = 0) where T : DeckAgent
        {
            return RentAgent(_agentByType[typeof(T)], uniqueId);
        }

        public DeckAgent RentAgent(int prefabId, int uniqueId = 0)
        {
            if (_agentPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.OnSpawned();

                if (uniqueId == 0)
                {
                    instance.UniqueId.ResetId(true);
                }
                else
                {
                    instance.SetNewUniqueId(uniqueId);
                }

                instance.gameObject.SetActive(true);
                return instance;
            }

            return CreateNewAgentInstance(prefabId, uniqueId);
        }

        public T[] BulkRentAgent<T>(DeckId prefabId, int count) where T : DeckAgent
        {
            return BulkRentAgent<T>(prefabId.Id, count);
        }

        public T[] BulkRentAgent<T>(int prefabId, int count) where T : DeckAgent
        {
            var result = new T[count];
            var counter = 0;

            if (_agentPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                while (counter < count)
                {
                    if (pool.Count > 0)
                    {
                        var instance = (T)pool.Pop();
                        instance.gameObject.SetActive(true);
                        instance.SetNewUniqueId();
                        instance.OnSpawned();
                        result[counter++] = instance;
                    }
                    else
                    {
                        var newInstance = CreateNewAgentInstance(prefabId);
                        result[counter++] = (T)newInstance;
                    }
                }
            }
            else
            {
                while (counter < count)
                {
                    var newInstance = CreateNewAgentInstance(prefabId);
                    result[counter++] = (T)newInstance;
                }
            }

            return result;
        }


        public DeckAgent[] BulkRentAgent(int prefabId, int[] uniqueIds)
        {
            var count = uniqueIds.Length;
            var result = new DeckAgent[count];
            var counter = 0;

            if (_agentPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                while (counter < count)
                {
                    if (pool.Count > 0)
                    {
                        var instance = pool.Pop();
                        instance.gameObject.SetActive(true);
                        var uniqueId = uniqueIds[counter];
                        if (uniqueId != 0)
                        {
                            instance.SetNewUniqueId(uniqueId);
                        }
                        else
                        {
                            instance.SetNewUniqueId();
                        }

                        instance.OnSpawned();
                        result[counter++] = instance;
                    }
                    else
                    {
                        var newInstance = CreateNewAgentInstance(prefabId);
                        result[counter++] = newInstance;
                    }
                }
            }
            else
            {
                while (counter < count)
                {
                    var newInstance = CreateNewAgentInstance(prefabId);


                    var uniqueId = uniqueIds[counter];
                    if (uniqueId != 0)
                    {
                        newInstance.SetNewUniqueId(uniqueId);
                    }
                    else
                    {
                        newInstance.SetNewUniqueId();
                    }

                    result[counter++] = newInstance;
                }
            }

            return result;
        }

        private DeckAgent CreateNewAgentInstance(int prefabId, int uniqueId = 0)
        {
            var agentPrefab = GetAgentById(prefabId);
            var newInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckAgent>();
            if (uniqueId == 0)
            {
                newInstance.Initialize();
            }
            else
            {
                newInstance.Initialize(uniqueId);
            }

            newInstance.OnSpawned();
            newInstance.gameObject.SetActive(true);

            return newInstance;
        }

        public void ReturnAgent(DeckAgent instance)
        {
            if (!_agentPool.TryGetValue(instance.PrefabId.Id, out var pool))
            {
                DeckLogger.Error("Unregistered agent tried to return");
                return;
            }

            instance.gameObject.SetActive(false);
            instance.OnDeSpawned();
            instance.transform.parent = _agentContainer;
            pool.Push(instance);
        }

        public DeckItemVisual RentItemVisual(DeckId id, int uniqueId = 0)
        {
            return RentItemVisual(id.Id, uniqueId);
        }

        public DeckItemVisual[] BulkRentItemVisual(DeckId id, int count)
        {
            return BulkRentItemVisual(id.Id, count);
        }

        public T RentItemVisual<T>(int uniqueId = 0) where T : DeckItemVisual
        {
            return (T)RentItemVisual(_itemVisualByType[typeof(T)], uniqueId);
        }

        public DeckItemVisual RentItemVisual(int prefabId, int uniqueId = 0)
        {
            if (_itemVisualPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.gameObject.SetActive(true);
                if (uniqueId == 0)
                {
                    instance.SetNewUniqueId();
                }

                instance.OnSpawned();
                return instance;
            }

            return CreateNewItemVisualInstance(prefabId);
        }

        private DeckItemVisual[] BulkRentItemVisual(int prefabId, int count)
        {
            var result = new DeckItemVisual[count];
            var counter = 0;

            if (_itemVisualPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                while (counter < count)
                {
                    if (pool.Count > 0)
                    {
                        var instance = pool.Pop();
                        instance.gameObject.SetActive(true);
                        instance.SetNewUniqueId();
                        instance.OnSpawned();
                        result[counter++] = instance;
                    }
                    else
                    {
                        var newInstance = CreateNewItemVisualInstance(prefabId);
                        result[counter++] = newInstance;
                    }
                }
            }
            else
            {
                while (counter < count)
                {
                    var newInstance = CreateNewItemVisualInstance(prefabId);
                    result[counter++] = newInstance;
                }
            }

            return result;
        }

        private DeckItemVisual CreateNewItemVisualInstance(int prefabId, int uniqueId = 0)
        {
            var itemVisualPrefab = GetItemVisualById(prefabId);
            var newInstance = _container.InstantiatePrefab(itemVisualPrefab).GetComponent<DeckItemVisual>();
            if (uniqueId == 0)
            {
                newInstance.SetNewUniqueId();
            }
            else
            {
                newInstance.SetUniqueId(new DeckId(uniqueId));
            }

            newInstance.gameObject.SetActive(true);
            newInstance.OnSpawned();
            return newInstance;
        }

        public void ReturnItemVisual(DeckItemVisual instance)
        {
            if (!_itemVisualPool.TryGetValue(instance.PrefabId.Id, out var pool))
            {
                DeckLogger.Error("Unsubscribed item tried to return");
                return;
            }

            instance.gameObject.SetActive(false);
            instance.OnDeSpawned();
            instance.transform.parent = _itemVisualContainer;
            pool.Push(instance);
        }

        public void ReturnItemVisual(DeckItemVisual[] instances)
        {
            Debug.LogError("Trying to return");
            var sample = instances[0];
            if (!_itemVisualPool.TryGetValue(sample.PrefabId.Id, out var pool))
            {
                DeckLogger.Error("Unsubscribed item tried to return");
                return;
            }

            foreach (var instance in instances)
            {
                instance.gameObject.SetActive(false);
                instance.OnDeSpawned();
                instance.transform.parent = _itemVisualContainer;
                pool.Push(instance);
            }
        }

        public DeckUIElement RentUIElement(Type t)
        {
            return RentUIElement(_uiElementByType[t].Id);
        }

        public T RentUIElement<T>() where T : DeckUIElement
        {
            return (T)RentUIElement(_uiElementByType[typeof(T)].Id);
        }

        public T[] RentUIElement<T>(int count) where T : DeckUIElement
        {
            var prefabId = _uiElementByType[typeof(T)].Id;
            var result = new T[count];
            var counter = 0;

            if (_uiElementPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                while (counter < count)
                {
                    if (pool.Count > 0)
                    {
                        var instance = pool.Pop() as T;
                        instance.gameObject.SetActive(true);
                        instance.OnSpawned();
                        result[counter++] = instance;
                    }
                    else
                    {
                        var uiElementPrefab = GetUIElementById(prefabId);
                        var newInstance = _container.InstantiatePrefab(uiElementPrefab).GetComponent<T>();

                        newInstance.gameObject.SetActive(true);
                        newInstance.OnSpawned();
                        result[counter++] = newInstance;
                    }
                }
            }
            else
            {
                while (counter < count)
                {
                    var uiElementPrefab = GetUIElementById(prefabId);
                    var newInstance = _container.InstantiatePrefab(uiElementPrefab).GetComponent<T>();

                    newInstance.gameObject.SetActive(true);
                    newInstance.OnSpawned();
                    result[counter++] = newInstance;
                }
            }

            return result;
        }

        private DeckUIElement RentUIElement(int prefabId)
        {
            if (_uiElementPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.gameObject.SetActive(true);
                instance.OnSpawned();
                return instance;
            }

            var uiElementPrefab = GetUIElementById(prefabId);
            var newInstance = _container.InstantiatePrefab(uiElementPrefab).GetComponent<DeckUIElement>();

            newInstance.gameObject.SetActive(true);
            newInstance.OnSpawned();

            return newInstance;
        }

        public void ReturnUIElement<T>(List<T> instances) where T : DeckUIElement
        {
            if (instances.IsNullOrEmpty())
            {
                DeckLogger.Error("Null or empty list of UI elements returned");
                return;
            }

            var sample = instances[0];
            var sampleId = sample.PrefabId.Id;
            if (!_uiElementPool.TryGetValue(sampleId, out var pool))
            {
                DeckLogger.Error("Unsubscribed UI element tried to return");
                return;
            }

            foreach (var instance in instances)
            {
                instance.gameObject.SetActive(false);
                instance.OnDeSpawned();
                instance.transform.parent = _uiContainer;
                pool.Push(instance);
            }
        }

        public void ReturnUIElement(DeckUIElement instance)
        {
            if (!_uiElementPool.TryGetValue(instance.PrefabId.Id, out var pool))
            {
                DeckLogger.Error("Unsubscribed UI element tried to return");
                return;
            }

            instance.gameObject.SetActive(false);
            instance.OnDeSpawned();
            instance.transform.SetParent(_uiContainer);
            pool.Push(instance);
        }

        private DeckAgent GetAgentById(int id)
        {
            return _agentDictionary[id];
        }

        private DeckItemVisual GetItemVisualById(int id)
        {
            return _itemVisualDictionary[id];
        }

        private DeckUIElement GetUIElementById(int id)
        {
            return _uiElementDictionary[id];
        }

        public List<DeckItemVisual> GetItemVisuals()
        {
            return itemVisuals;
        }

        public List<DeckAgent> GetAgents()
        {
            return agents;
        }

        public List<DeckUIElement> GetUIElements()
        {
            return uiElements;
        }
    }
}