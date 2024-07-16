using System.Collections.Generic;
using Deck.Agent;
using Deck.Item;
using Deck.UI.InGame;
using Deck.Utility.Poolable;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif
using UnityEngine;
using Zenject;

namespace Deck.Save.Data
{
    [CreateAssetMenu(menuName = "Deck/Data/Resolver/Instance creator", fileName = "Instance creator")]
    public class DeckInstanceCreator : ScriptableObject
    {
        [SerializeField] private List<DeckAgent> agents;
        [SerializeField] private List<DeckItemVisual> itemVisuals;
        [SerializeField] private List<DeckUIElement> uiElements;

        private Dictionary<int, DeckAgent> _agentDictionary;
        private Dictionary<int, DeckItemVisual> _itemVisualDictionary;
        private Dictionary<int, DeckUIElement> _uiElementDictionary;
        private bool _initialized;
        private DiContainer _container;

#if UNITY_EDITOR
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            var instanceCreator = Resources.Load<DeckInstanceCreator>("Data/Resolver/Deck Instance Creator");
            instanceCreator.itemVisuals = new List<DeckItemVisual>();
            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckItemVisual)))
            {
                if (PrefabUtility.GetPrefabParent(itemVisual) == null && !PrefabUtility.IsPartOfPrefabAsset(itemVisual))
                {
                    continue;
                }

                var element = itemVisual as DeckItemVisual;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                instanceCreator.itemVisuals.Add(itemVisual as DeckItemVisual);
            }

            instanceCreator.agents = new List<DeckAgent>();
            foreach (var agent in Resources.FindObjectsOfTypeAll(typeof(DeckAgent)))
            {
                if (PrefabUtility.GetPrefabParent(agent) == null && !PrefabUtility.IsPartOfPrefabAsset(agent))
                {
                    continue;
                }

                var element = agent as DeckAgent;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                instanceCreator.agents.Add(agent as DeckAgent);
            }

            instanceCreator.uiElements = new List<DeckUIElement>();
            foreach (var uiElement in Resources.FindObjectsOfTypeAll(typeof(DeckUIElement)))
            {
                if (PrefabUtility.GetPrefabParent(uiElement) == null && !PrefabUtility.IsPartOfPrefabAsset(uiElement))
                {
                    continue;
                }

                var element = uiElement as DeckUIElement;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                instanceCreator.uiElements.Add(uiElement as DeckUIElement);
            }
        }

        [Button]
        private void OnValidate()
        {
            itemVisuals = new List<DeckItemVisual>();
            foreach (var itemVisual in Resources.FindObjectsOfTypeAll(typeof(DeckItemVisual)))
            {
                if (PrefabUtility.GetPrefabParent(itemVisual) == null && !PrefabUtility.IsPartOfPrefabAsset(itemVisual))
                {
                    continue;
                }

                var element = itemVisual as DeckItemVisual;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                itemVisuals.Add(itemVisual as DeckItemVisual);
            }

            agents = new List<DeckAgent>();
            foreach (var agent in Resources.FindObjectsOfTypeAll(typeof(DeckAgent)))
            {
                if (PrefabUtility.GetPrefabParent(agent) == null && !PrefabUtility.IsPartOfPrefabAsset(agent))
                {
                    continue;
                }

                var element = agent as DeckAgent;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                agents.Add(agent as DeckAgent);
            }

            uiElements = new List<DeckUIElement>();
            foreach (var uiElement in Resources.FindObjectsOfTypeAll(typeof(DeckUIElement)))
            {
                if (PrefabUtility.GetPrefabParent(uiElement) == null && !PrefabUtility.IsPartOfPrefabAsset(uiElement))
                {
                    continue;
                }

                var element = uiElement as DeckUIElement;

                if (!string.IsNullOrEmpty(element.gameObject.scene.name))
                {
                    continue;
                }

                uiElements.Add(uiElement as DeckUIElement);
            }
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
            if (_initialized)
            {
                return;
            }

            _agentDictionary = new Dictionary<int, DeckAgent>();
            foreach (var deckAgent in agents)
            {
                _agentDictionary[deckAgent.PrefabId.ID] = deckAgent;
            }

            _itemVisualDictionary = new Dictionary<int, DeckItemVisual>();
            foreach (var itemVisual in itemVisuals)
            {
                _itemVisualDictionary[itemVisual.PrefabId.ID] = itemVisual;
            }

            _uiElementDictionary = new Dictionary<int, DeckUIElement>();
            foreach (var uielement in uiElements)
            {
                _uiElementDictionary[uielement.PrefabId.ID] = uielement;
            }
        }

        public DeckAgent GetAgentById(int id)
        {
            return _agentDictionary[id];
        }

        public DeckItemVisual GetItemVisualById(int id)
        {
            return _itemVisualDictionary[id];
        }

        public DeckUIElement GetUIElementById(int id)
        {
            return _uiElementDictionary[id];
        }

        public DeckAgent CreateNewAgentInstance(int prefabId, int uniqueId = 0)
        {
            var agentPrefab = GetAgentById(prefabId);
            var agentInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckAgent>();
            if (uniqueId == 0)
            {
                agentInstance.Initialize();
            }
            else
            {
                agentInstance.Initialize(uniqueId);
            }

            return agentInstance;
        }

        public DeckItemVisual CreateNewItemVisualInstance(int prefabId, int uniqueId = 0)
        {
            var itemVisualPrefab = GetItemVisualById(prefabId);
            var itemVisualInstance = _container.InstantiatePrefab(itemVisualPrefab).GetComponent<DeckItemVisual>();
            if (uniqueId == 0)
            {
                itemVisualInstance.SetNewUniqueId();
            }
            else
            {
                itemVisualInstance.SetUniqueId(new DeckId(uniqueId));
            }

            return itemVisualInstance;
        }


        public DeckUIElement CreateNewUIElement(int prefabId)
        {
            var agentPrefab = GetUIElementById(prefabId);
            var uiElement = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckUIElement>();

            return uiElement;
        }
    }
}