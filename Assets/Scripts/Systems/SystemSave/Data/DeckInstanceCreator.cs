using System.Collections.Generic;
using Deck.Agent;
using Deck.Item;
using Deck.UI.InGame;
#if UNITY_EDITOR
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

        private Dictionary<int, DeckAgent> _agentDictionary;
        private Dictionary<int, DeckItemVisual> _itemVisualDictionary;
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
                instanceCreator.itemVisuals.Add(itemVisual as DeckItemVisual);
            }

            instanceCreator.agents = new List<DeckAgent>();
            foreach (var agent in Resources.FindObjectsOfTypeAll(typeof(DeckAgent)))
            {
                instanceCreator.agents.Add(agent as DeckAgent);
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
        }

        public DeckAgent GetAgentById(int id)
        {
            return _agentDictionary[id];
        }

        public DeckItemVisual GetItemVisualById(int id)
        {
            return _itemVisualDictionary[id];
        }

        public DeckAgent CreateNewAgentInstance(int prefabId, int agentGuid = 0)
        {
            var agentPrefab = GetAgentById(prefabId);
            var agentInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckAgent>();
            if (agentGuid == 0)
            {
                agentInstance.Initialize();
            }
            else
            {
                agentInstance.Initialize(agentGuid);
            }

            return agentInstance;
        }

        public DeckItemVisual CreateNewItemVisualInstance(int prefabId, int itemGuid = 0)
        {
            var itemVisualPrefab = GetItemVisualById(prefabId);
            var itemVisualInstance = _container.InstantiatePrefab(itemVisualPrefab).GetComponent<DeckItemVisual>();
            if (itemGuid == 0)
            {
                itemVisualInstance.SetNewUniqueId();
            }
            else
            {
                itemVisualInstance.SetUniqueId(new DeckId(itemGuid));
            }

            return itemVisualInstance;
        }
    }
}