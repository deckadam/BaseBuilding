using System;
using System.Collections.Generic;
using Deck.Services;

namespace Services.Implementations.Tables
{
    public class DeckServiceUsableTableController : DeckServiceBase
    {
        private List<DeckTableWithChairs> _usableTables;
        
        public override void AfterGameSessionInitialized()
        {
        }

        public override void BeforeGameSessionDeinitialized()
        {
        }
    }

    [Serializable]
    public class DeckTableWithChairs
    {
        
    }
}