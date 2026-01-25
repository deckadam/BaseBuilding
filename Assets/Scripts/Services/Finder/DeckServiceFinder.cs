using System;
using System.Collections.Generic;
using Base;
using Instancing;
using Sirenix.Utilities;
using Zenject;

namespace Services.Finder
{
    public class DeckServiceFinder : DeckServiceBase
    {
        private Dictionary<int, DeckAgent> _agentsWithUniqueId = new();
        private Dictionary<int, HashSet<DeckAgent>> _agentsWithPrefabId = new();
        private Dictionary<int, DeckItemVisual> _itemVisualsWithUniqueId = new();
        private Dictionary<int, HashSet<DeckItemVisual>> _itemVisualsWithPrefabId = new();
        private Dictionary<DeckActionTag, HashSet<DeckAgent>> _agentsWithActions = new();
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public override void Initialize()
        {
            foreach (var suit in (DeckActionTag[])Enum.GetValues(typeof(DeckActionTag)))
            {
                _agentsWithActions.Add(suit, new HashSet<DeckAgent>());
            }
        }

        public void RegisterItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisualsWithUniqueId[itemVisual.UniqueId.ID] = itemVisual;

            if (!_itemVisualsWithPrefabId.TryGetValue(itemVisual.PrefabId.ID, out var list))
            {
                list = new HashSet<DeckItemVisual>();
                _itemVisualsWithPrefabId.Add(itemVisual.PrefabId.ID, list);
            }

            _itemVisualsWithPrefabId[itemVisual.PrefabId.ID].Add(itemVisual);
        }

        public void RegisterItemVisual(DeckItemVisual[] itemVisuals)
        {
            if (itemVisuals.IsNullOrEmpty())
            {
                return;
            }

            foreach (var item in itemVisuals)
            {
                _itemVisualsWithUniqueId[item.UniqueId.ID] = item;
            }

            var sample = itemVisuals[0];
            if (!_itemVisualsWithPrefabId.TryGetValue(sample.PrefabId.ID, out var list))
            {
                list = new HashSet<DeckItemVisual>();
                _itemVisualsWithPrefabId.Add(sample.PrefabId.ID, list);
            }

            _itemVisualsWithPrefabId[sample.PrefabId.ID].AddRange(itemVisuals);
        }

        public void RemoveItemVisual(DeckItemVisual itemVisual)
        {
            _itemVisualsWithUniqueId.Remove(itemVisual.UniqueId.ID);
            _itemVisualsWithPrefabId[itemVisual.PrefabId.ID].Remove(itemVisual);
        }

        public void RemoveItemVisual(DeckItemVisual[] itemVisual)
        {
            foreach (var instance in itemVisual)
            {
                _itemVisualsWithUniqueId.Remove(instance.UniqueId.ID);
                _itemVisualsWithPrefabId[instance.PrefabId.ID].Remove(instance);
            }
        }

        public DeckItemVisual GetItemVisual(int uniqueId)
        {
            return _itemVisualsWithUniqueId[uniqueId];
        }

        public IEnumerable<DeckItemVisual> GetItemVisuals()
        {
            return _itemVisualsWithUniqueId.Values;
        }

        public bool TryGetItemVisualsWithPrefabId(DeckId prefabId, out IReadOnlyCollection<DeckItemVisual> itemVisuals)
        {
            if (!_itemVisualsWithPrefabId.TryGetValue(prefabId.ID, out var list))
            {
                itemVisuals = null;
                return false;
            }

            itemVisuals = list;
            return true;
        }

        public void RegisterAgent(DeckAgent agent)
        {
            _agentsWithUniqueId[agent.UniqueId.ID] = agent;
            var tags = agent.GetTags();
            foreach (var deckActionTag in tags)
            {
                _agentsWithActions[deckActionTag].Add(agent);
            }

            if (!_agentsWithPrefabId.TryGetValue(agent.PrefabId.ID, out var list))
            {
                list = new HashSet<DeckAgent>();
                _agentsWithPrefabId.Add(agent.PrefabId.ID, list);
            }

            _agentsWithPrefabId[agent.PrefabId.ID].Add(agent);
        }

        public void RemoveAgent(DeckAgent agent)
        {
            _agentsWithUniqueId.Remove(agent.UniqueId.ID);
            _agentsWithPrefabId[agent.PrefabId.ID].Remove(agent);

            var tags = agent.GetTags();
            foreach (var deckActionTag in tags)
            {
                _agentsWithActions[deckActionTag].Remove(agent);
            }
        }

        public DeckAgent GetAgent(int uniqueId)
        {
            return _agentsWithUniqueId[uniqueId];
        }

        public HashSet<DeckAgent> GetAgentsWithTag(DeckActionTag tag)
        {
            return _agentsWithActions[tag];
        }

        public IEnumerable<DeckAgent> GetAgents()
        {
            return _agentsWithUniqueId.Values;
        }

        public bool TryGetAgentsWithPrefabId(DeckId prefabId, out IReadOnlyCollection<DeckAgent> agents)
        {
            if (!_agentsWithPrefabId.TryGetValue(prefabId.ID, out var list))
            {
                agents = null;
                return false;
            }

            agents = list;
            return true;
        }

        public override void BeforeGameSessionDeinitialized()
        {
            foreach (var agent in _agentsWithUniqueId.Values)
            {
                _instanceProvider.ReturnAgent(agent);
            }

            foreach (var itemVisual in _itemVisualsWithUniqueId.Values)
            {
                _instanceProvider.ReturnItemVisual(itemVisual);
            }

            _agentsWithUniqueId.Clear();
            _itemVisualsWithUniqueId.Clear();
        }
    }
}