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
        private Dictionary<DeckAgentTable, DeckTableWithChairs> _usableTables;

        private HashSet<DeckAgentTable> _tables;
        private HashSet<DeckAgentChair> _chairs;

        public override void Initialize()
        {
            _usableTables = new Dictionary<DeckAgentTable, DeckTableWithChairs>();
            _tables = new HashSet<DeckAgentTable>();
            _chairs = new HashSet<DeckAgentChair>();

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
            _chairs.Remove(obj.Chair);
            foreach (var keyValuePair in _usableTables)
            {
                keyValuePair.Value.RemoveChair(obj.Chair);
            }
        }

        private void OnChairPlaced(DeckEventOnChairPlaced obj)
        {
            _chairs.Add(obj.Chair);

            foreach (var table in _usableTables)
            {
                table.Value.AddChair(obj.Chair);
                DeckEventOnTableAvailable.Create(table.Value).Send();
            }
        }

        private void OnTableDestroyed(DeckEventOnTableDestroyed obj)
        {
            _tables.Remove(obj.Table);
            RemoveTable(obj.Table);
        }

        private void OnTablePlaced(DeckEventOnTablePlaced obj)
        {
            _tables.Add(obj.Table);
            var newTableWithChairs = new DeckTableWithChairs(obj.Table);
            _usableTables.Add(obj.Table, newTableWithChairs);

            DeckEventOnTableAvailable.Create(newTableWithChairs).Send();
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