using System.Collections.Generic;
using Deck.Agent;
using Deck.Item;
using Deck.Services;
using UnityEngine;

namespace Services.AgentFinder
{
    public class DeckServiceFinder : DeckServiceBase
    {
        private Dictionary<int, DeckAgent> _agents = new();
        private Dictionary<int, DeckItemVisual> _itemVisuals = new();

        public void RegisterItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals[itemVisual.UniqueId.ID] = itemVisual;
        }

        public void RemoveItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals.Remove(itemVisual.UniqueId.ID);
        }

        public DeckItemVisual GetItemVisual(int uniqueId)
        {
            return _itemVisuals[uniqueId];
        }
        
        public IEnumerable<DeckItemVisual> GetItemVisuals()
        {
            return _itemVisuals.Values;
        }

        public void RegisterAgent(DeckAgent agent)
        {
            _agents[agent.GetUniqueId().ID] = agent;
        }

        public void RemoveAgent(DeckAgent agent)
        {
            _agents.Remove(agent.GetUniqueId().ID);
        }

        public DeckAgent GetAgent(int uniqueId)
        {
            return _agents[uniqueId];
        }
        
        public IEnumerable<DeckAgent> GetAgents()
        {
            return _agents.Values;
        }
    }
}