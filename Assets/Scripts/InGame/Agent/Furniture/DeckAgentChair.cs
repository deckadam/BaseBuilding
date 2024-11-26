using Deck.Base;
using Deck.Components.Building;
using Deck.Services.Tables.Events;
using Deck.Utility;

namespace Deck.Components.Furniture
{
    public class DeckAgentChair : DeckBuilding
    {
        private DeckAgentHumanoid _humanoid;
        private bool _isAvailable;
        
        public bool IsAvailable => _humanoid == null;

        protected override void AfterInitialize()
        {
            DeckEventOnChairPlaced.Create(this).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnChairDestroyed.Create(this).Send();
        }

        public void SetOccupied()
        {
            _isAvailable = false;
        }

        public void SetUnoccupied()
        {
            _isAvailable = true;
        }

        public void OnTableDestroyed()
        {
            if (_isAvailable)
            {
                return;
            }

            _humanoid.GetUp();
        }

    }
}