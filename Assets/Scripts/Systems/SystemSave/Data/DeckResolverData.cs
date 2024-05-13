using System;
using System.Collections.Generic;
using Deck.Agent;
using UnityEngine;

namespace Deck.Save.Data
{
    [CreateAssetMenu(menuName = "Deck/Data/Resolver/Resolver Data", fileName = "Deck Resolver Data")]
    public class DeckResolverData : ScriptableObject
    {
        [SerializeField] private List<DeckAgent> agents;

        private Dictionary<Guid, DeckAgent> _agentDictionary;

        private bool _initialized;

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _agentDictionary = new Dictionary<Guid, DeckAgent>();
            foreach (var deckAgent in agents)
            {
                _agentDictionary[Guid.Parse(deckAgent.GetPrefabId())] = deckAgent;
            }
        }

        public DeckAgent GetAgentById(Guid id)
        {
            Initialize();
            return _agentDictionary[id];
        }
    }
}