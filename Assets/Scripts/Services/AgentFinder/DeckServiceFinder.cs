using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Item;
using Deck.Services;
using UnityEngine;

namespace Services.AgentFinder
{
    public class DeckServiceFinder : DeckServiceBase
    {
        private Dictionary<Guid, DeckAgent> _agents = new();
        private Dictionary<Guid, DeckItemVisual> _itemVisuals = new();

        public void RegisterItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals[itemVisual.UniqueId.ID] = itemVisual;
        }

        public void RemoveItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals.Remove(itemVisual.UniqueId.ID);
        }

        public DeckItemVisual GetItemVisual(Guid uniqueId)
        {
            return _itemVisuals[uniqueId];
        }

        public DeckItemVisual GetItemVisual(string uniqueId)
        {
            var guid = new Guid(uniqueId);
            return _itemVisuals[guid];
        }

        public void RegisterAgent(DeckAgent agent)
        {
            _agents[agent.GetUniqueId().ID] = agent;
        }

        public void RemoveAgent(DeckAgent agent)
        {
            _agents.Remove(agent.GetUniqueId().ID);
        }

        public DeckAgent GetAgent(Guid uniqueId)
        {
            return _agents[uniqueId];
        }

        public DeckAgent GetAgent(string uniqueId)
        {
            Debug.LogError(uniqueId);
            var guid = new Guid(uniqueId);
            return _agents[guid];
        }
    }
}