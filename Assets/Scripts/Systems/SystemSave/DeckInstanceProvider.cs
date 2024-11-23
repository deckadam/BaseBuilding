using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Base.Id;
using Deck.Components;
using Deck.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace Deck.Save
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

                var instance = agent as DeckAgent;

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

                var instance = itemVisual as DeckItemVisual;

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

                var instance = uiElement as DeckUIElement;

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
                _agentDictionary[agent.PrefabId.ID] = agent;
                _agentPool[agent.PrefabId.ID] = new Stack<DeckAgent>();
                _agentByType[agent.GetType()] = agent.PrefabId;
            }

            _itemVisualDictionary = new Dictionary<int, DeckItemVisual>();
            _itemVisualPool = new Dictionary<int, Stack<DeckItemVisual>>();
            _itemVisualByType = new Dictionary<Type, DeckId>();
            foreach (var itemVisual in itemVisuals)
            {
                _itemVisualDictionary[itemVisual.PrefabId.ID] = itemVisual;
                _itemVisualPool[itemVisual.PrefabId.ID] = new Stack<DeckItemVisual>();
                _itemVisualByType[itemVisual.GetType()] = itemVisual.PrefabId;
            }

            _uiElementDictionary = new Dictionary<int, DeckUIElement>();
            _uiElementPool = new Dictionary<int, Stack<DeckUIElement>>();
            _uiElementByType = new Dictionary<Type, DeckId>();
            foreach (var uielement in uiElements)
            {
                _uiElementDictionary[uielement.PrefabId.ID] = uielement;
                _uiElementPool[uielement.PrefabId.ID] = new Stack<DeckUIElement>();
                _uiElementByType[uielement.GetType()] = uielement.PrefabId;
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
            return RentAgent(id.ID, uniqueId);
        }

        public DeckAgent RentAgent<T>(int uniqueId = 0) where T : DeckUIElement
        {
            return RentAgent(_agentByType[typeof(T)], uniqueId);
        }

        public DeckAgent RentAgent(int prefabId, int uniqueId = 0)
        {
            if (_agentPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.OnSpawned();
                instance.gameObject.SetActive(true);
                return instance;
            }

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
            if (_agentPool.TryGetValue(instance.PrefabId.ID, out var pool))
            {
                instance.gameObject.SetActive(false);
                instance.OnDespawned();
                instance.transform.parent = _agentContainer;
                pool.Push(instance);
            }
        }

        public DeckItemVisual RentItemVisual(DeckId id, int uniqueId = 0)
        {
            return RentItemVisual(id.ID, uniqueId);
        }

        public T RentItemVisual<T>(int uniqueId = 0) where T : DeckItemVisual
        {
            return RentItemVisual(_itemVisualByType[typeof(T)], uniqueId) as T;
        }

        public DeckItemVisual RentItemVisual(int prefabId, int uniqueId = 0)
        {
            if (_itemVisualPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.gameObject.SetActive(true);
                instance.OnSpawned();
                return instance;
            }

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
            if (_itemVisualPool.TryGetValue(instance.PrefabId.ID, out var pool))
            {
                instance.gameObject.SetActive(false);
                instance.OnDespawned();
                instance.transform.parent = _itemVisualContainer;
                pool.Push(instance);
            }
        }

        public DeckUIElement RentUIElement(DeckId id, int uniqueId = 0)
        {
            return RentUIElement(id.ID, uniqueId);
        }

        public DeckUIElement RentUIElement(Type t, int uniqueId = 0)
        {
            return RentUIElement(_uiElementByType[t], uniqueId);
        }

        public T RentUIElement<T>(int uniqueId = 0) where T : DeckUIElement
        {
            return RentUIElement(_uiElementByType[typeof(T)], uniqueId) as T;
        }

        public DeckUIElement RentUIElement(int prefabId, int uniqueId = 0)
        {
            if (_uiElementPool.TryGetValue(prefabId, out var pool) && pool.Count > 0)
            {
                var instance = pool.Pop();
                instance.gameObject.SetActive(true);
                instance.OnSpawned();
                return instance;
            }

            var agentPrefab = GetUIElementById(prefabId);
            var newInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckUIElement>();

            newInstance.gameObject.SetActive(true);
            newInstance.OnSpawned();

            return newInstance;
        }

        public void ReturnUIElement<T>(List<T> instances) where T : DeckUIElement
        {
            if (instances == null)
            {
                DeckLogger.Error("Null UI element returned");
                return;
            }

            if (instances.Count == 0)
            {
                return;
            }

            var id = instances[0].PrefabId.ID;
            if (!_uiElementPool.TryGetValue(id, out var pool)) return;
            foreach (var instance in instances)
            {
                instance.gameObject.SetActive(false);
                instance.OnDespawned();
                instance.transform.parent = _uiContainer;
                pool.Push(instance);
            }
        }

        public void ReturnUIElement(DeckUIElement instance)
        {
            if (_uiElementPool.TryGetValue(instance.PrefabId.ID, out var pool))
            {
                instance.gameObject.SetActive(false);
                instance.OnDespawned();
                instance.transform.parent = _uiContainer;
                pool.Push(instance);
            }
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