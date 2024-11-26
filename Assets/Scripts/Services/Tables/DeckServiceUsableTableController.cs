using System;
using System.Collections.Generic;
using Deck.Components.Furniture;
using Deck.EventManager;
using Deck.InGame.Agent.Furniture;
using Deck.Services.Tables.Events;
using Deck.Utility;
using UnityEngine;

namespace Deck.Services.Tables
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

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnTablePlaced>(OnTablePlaced);
            DeckEventManager.Unregister<DeckEventOnTableDestroyed>(OnTableDestroyed);
            DeckEventManager.Unregister<DeckEventOnChairPlaced>(OnChairPlaced);
            DeckEventManager.Unregister<DeckEventOnChairDestroyed>(OnChairDestroyed);
        }

        private void OnChairDestroyed(DeckEventOnChairDestroyed obj)
        {
            _freeChairs.Remove(obj.Chair);

            foreach (var keyValuePair in _usableTables)
            {
                keyValuePair.Value.RemoveChair(obj.Chair);
            }
        }

        private void OnChairPlaced(DeckEventOnChairPlaced obj)
        {
            _freeChairs.Add(obj.Chair);

            if (!TryGetBestMatch(obj.Chair, out var bestMatch))
            {
                _freeChairs.Add(obj.Chair);
                return;
            }

            bestMatch.AddChair(obj.Chair);
            DeckEventOnTableAvailable.Create(bestMatch).Send();
        }


        private bool TryGetBestMatch(DeckAgentChair chair, out DeckTableWithChairs result)
        {
            result = null;

            var currentBestDistance = tableConnectionDistance * 2f;
            var currentBestDotProduct = -1;
            var foundAMatch = false;
            foreach (var usableTable in _usableTables)
            {
                var tableTransform = usableTable.Value.Table.transform;
                var tableSize = usableTable.Value.Table.GetItemVisual().GetSize();
                var chairSize = chair.GetItemVisual().GetSize();

                var distance = tableTransform.position.Distance(chair.transform.position) - tableSize - chairSize;
                var dotProduct = Vector3.Dot(chair.transform.forward, (tableTransform.position - chair.transform.position).normalized);
                
                Debug.LogError(distance +"  "+ dotProduct);
                if (!(distance < tableConnectionDistance)) continue;
                if (dotProduct <= tableConnectionDotProduct) continue;

                var distanceDiff = currentBestDistance - distance;
                var dotProductDiff = distanceDiff - currentBestDotProduct;


                // Debug.LogError(distanceDiff + "  " + dotProductDiff);
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
            RemoveTable(obj.Table);
        }

        private void OnTablePlaced(DeckEventOnTablePlaced obj)
        {
            var newTableWithChairs = new DeckTableWithChairs(obj.Table);
            _usableTables.Add(obj.Table, newTableWithChairs);

            DeckEventOnTableAvailable.Create(newTableWithChairs).Send();

            var matchedChairs = new HashSet<DeckAgentChair>();
            foreach (var chair in _freeChairs)
            {
                if (!TryGetBestMatch(chair, out var bestMatch))
                {
                    continue;
                }

                matchedChairs.Add(chair);
                Debug.LogError("has match");
                bestMatch.AddChair(chair);
                DeckEventOnTableAvailable.Create(bestMatch).Send();
            }

            foreach (var chair in matchedChairs)
            {
                _freeChairs.Remove(chair);
            }
        }

        private void RemoveTable(DeckAgentTable table)
        {
            foreach (var chair in _usableTables[table].Chairs)
            {
                chair.OnTableDestroyed();
            }

            _usableTables.Remove(table);
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
        public DeckAgentTable Table;
        public HashSet<DeckAgentChair> Chairs;

        public DeckTableWithChairs(DeckAgentTable table)
        {
            Table = table;
            Chairs = new HashSet<DeckAgentChair>();
        }

        public void AddChair(DeckAgentChair chair)
        {
            Chairs.Add(chair);
        }

        public void RemoveChair(DeckAgentChair chair)
        {
            Chairs.Remove(chair);
        }
    }
}