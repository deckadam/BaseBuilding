using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Item;
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

        [Inject]
        private void Inject(DiContainer container)
        {
            _container = container;
        }

        public void Initialize()
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
            Initialize();
            return _agentDictionary[id];
        }

        public DeckAgent CreateNewAgentInstance(int prefabId, int agentGuid)
        {
            var agentPrefab = GetAgentById(prefabId);
            var agentInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckAgent>();
            agentInstance.Initialize(agentGuid);
            return agentInstance;
        }

        public DeckItemVisual GetItemVisualById(Guid id)
        {
            return null;
        }
    }
}