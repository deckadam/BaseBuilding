using Deck.Component;
using Deck.Components;
using Deck.Data.Agent;
using Deck.Inventory;
using Deck.Test.General;

namespace Deck.Player
{
    public abstract class DeckAgent : DeckComponentHolder, IDeckSelectable
    {
        protected DeckAgentData _data;
        public abstract DeckSelectOperations[] GetAvailableOperations();

        public abstract void OnPossesStarted();
        public abstract void OnPossesFinished();
        public abstract DeckInventoryComponent GetInventory();
        public abstract DeckHealthComponent GetHealthComponent();
        
    }
}