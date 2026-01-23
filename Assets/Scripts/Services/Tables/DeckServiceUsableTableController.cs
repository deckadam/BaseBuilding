using System;
using System.Collections.Generic;
using EventManager;
using InGame.Agent.Furniture;
using Services.Tables.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Utility;

namespace Services.Tables
{
    public class DeckServiceUsableTableController : DeckServiceBase
    {
        [SerializeField] private float tableConnectionDistance;
        [SerializeField] private float tableConnectionDotProduct;
        private Dictionary<DeckAgentTable, DeckTableWithChairs> _usableTables;
        private HashSet<DeckAgentChair> _freeChairs;

        public override void Initialize()
        {
            _usableTables = new Dictionary<DeckAgentTable, DeckTableWithChairs>();
            _freeChairs = new HashSet<DeckAgentChair>();

            DeckEventManager.Register<DeckEventOnTablePlaced>(OnTablePlaced);
            DeckEventManager.Register<DeckEventOnTableDestroyed>(OnTableDestroyed);
            DeckEventManager.Register<DeckEventOnChairPlaced>(OnChairPlaced);
            DeckEventManager.Register<DeckEventOnChairDestroyed>(OnChairDestroyed);
        }

        protected override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnTablePlaced>(OnTablePlaced);
            DeckEventManager.Unregister<DeckEventOnTableDestroyed>(OnTableDestroyed);
            DeckEventManager.Unregister<DeckEventOnChairPlaced>(OnChairPlaced);
            DeckEventManager.Unregister<DeckEventOnChairDestroyed>(OnChairDestroyed);
        }

        private void OnChairDestroyed(DeckEventOnChairDestroyed obj)
        {
            _freeChairs.Remove(obj.AgentChair);

            foreach (var keyValuePair in _usableTables)
            {
                keyValuePair.Value.RemoveChair(obj.AgentChair);
            }
        }

        private void OnChairPlaced(DeckEventOnChairPlaced obj)
        {
            _freeChairs.Add(obj.AgentChair);

            if (!TryGetBestMatch(obj.AgentChair, out var bestMatch))
            {
                _freeChairs.Add(obj.AgentChair);
                return;
            }

            bestMatch.AddChair(obj.AgentChair);
            DeckEventOnTableAvailable.Create(bestMatch).Send();
        }


        private bool TryGetBestMatch(DeckAgentChair agentChair, out DeckTableWithChairs result)
        {
            result = null;

            var currentBestDistance = tableConnectionDistance * 2f;
            var currentBestDotProduct = -1;
            var foundAMatch = false;
            foreach (var usableTable in _usableTables)
            {
                var tableTransform = usableTable.Value.agentTable.transform;
                var tableSize = usableTable.Value.agentTable.GetItemVisual().GetSize();
                var chairSize = agentChair.GetItemVisual().GetSize();

                var distance = tableTransform.position.Distance(agentChair.transform.position) - tableSize - chairSize;
                var dotProduct = Vector3.Dot(agentChair.transform.forward, (tableTransform.position - agentChair.transform.position).normalized);

                if (!(distance < tableConnectionDistance)) continue;
                if (dotProduct <= tableConnectionDotProduct) continue;

                var distanceDiff = currentBestDistance - distance;
                var dotProductDiff = distanceDiff - currentBestDotProduct;


                if (distanceDiff + dotProductDiff > 0)
                {
                    result = usableTable.Value;
                    foundAMatch = true;
                }
            }

            return foundAMatch;
        }

        private void OnTableDestroyed(DeckEventOnTableDestroyed obj)
        {
            RemoveTable(obj.AgentTable);
        }

        private void OnTablePlaced(DeckEventOnTablePlaced obj)
        {
            var newTableWithChairs = new DeckTableWithChairs(obj.AgentTable);
            _usableTables.Add(obj.AgentTable, newTableWithChairs);

            DeckEventOnTableAvailable.Create(newTableWithChairs).Send();

            var matchedChairs = new HashSet<DeckAgentChair>();
            foreach (var chair in _freeChairs)
            {
                if (!TryGetBestMatch(chair, out var bestMatch))
                {
                    continue;
                }

                matchedChairs.Add(chair);
                bestMatch.AddChair(chair);
                DeckEventOnTableAvailable.Create(bestMatch).Send();
            }

            foreach (var chair in matchedChairs)
            {
                _freeChairs.Remove(chair);
            }
        }

        private void RemoveTable(DeckAgentTable agentTable)
        {
            foreach (var chair in _usableTables[agentTable].Chairs)
            {
                chair.OnTableDestroyed();
            }

            _usableTables.Remove(agentTable);
        }

        public override void AfterGameSessionInitialized()
        {
            _usableTables.Clear();
        }

        public override void BeforeGameSessionDeinitialized()
        {
        }
    }

    [Serializable]
    public class DeckTableWithChairs
    {
        [FormerlySerializedAs("Table")] public DeckAgentTable agentTable;
        public HashSet<DeckAgentChair> Chairs;

        public DeckTableWithChairs(DeckAgentTable agentTable)
        {
            this.agentTable = agentTable;
            Chairs = new HashSet<DeckAgentChair>();
        }

        public void AddChair(DeckAgentChair agentChair)
        {
            Chairs.Add(agentChair);
        }

        public void RemoveChair(DeckAgentChair agentChair)
        {
            Chairs.Remove(agentChair);
        }
    }
}