using System;
using System.Collections.Generic;
using Base;

namespace Services.Finder
{
    public class DeckServiceFinder : DeckServiceBase
    {
        private Dictionary<int, DeckAgent> _agents = new();
        private Dictionary<int, DeckItemVisual> _itemVisuals = new();
        private Dictionary<DeckActionTag, HashSet<DeckAgent>> _agentsWithActions = new();

        public override void Initialize()
        {
            foreach (var suit in (DeckActionTag[])Enum.GetValues(typeof(DeckActionTag)))
            {
                _agentsWithActions.Add(suit, new HashSet<DeckAgent>());
            }
        }

        public void RegisterItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals[itemVisual.UniqueId.ID] = itemVisual;
        }

        public void BulkRegisterItemVisual(DeckItemVisual[] itemVisual)
        {
            foreach (var item in itemVisual)
            {
                _itemVisuals[item.UniqueId.ID] = item;
            }
        }

        public void RemoveItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisuals.Remove(itemVisual.UniqueId.ID);
        }

        public void RemoveItemVisual(DeckItemVisual[] itemVisual)
        {
            foreach (var instance in itemVisual)
            {
                _itemVisuals.Remove(instance.UniqueId.ID);
            }
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
            _agents[agent.UniqueId.ID] = agent;
            var tags = agent.GetTags();
            foreach (var deckActionTag in tags)
            {
                _agentsWithActions[deckActionTag].Add(agent);
            }
        }

        public void RemoveAgent(DeckAgent agent)
        {
            _agents.Remove(agent.UniqueId.ID);

            var tags = agent.GetTags();
            foreach (var deckActionTag in tags)
            {
                _agentsWithActions[deckActionTag].Remove(agent);
            }
        }

        public DeckAgent GetAgent(int uniqueId)
        {
            return _agents[uniqueId];
        }

        public HashSet<DeckAgent> GetAgentsWithTag(DeckActionTag tag)
        {
            return _agentsWithActions[tag];
        }

        public IEnumerable<DeckAgent> GetAgents()
        {
            return _agents.Values;
        }
    }
}