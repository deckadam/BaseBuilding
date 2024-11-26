using Deck.Base;
using Deck.Components.Building;
using Deck.Services.Tables.Events;
using Deck.Utility;

namespace Deck.Components.Furniture
{
    public class DeckAgentChair : DeckBuilding
    {
        public bool IsAvailable { get; private set; } = true;

        private DeckAgentHumanoid _humanoid;

        protected override void InternalAfterBuildingInitialized()
        {
            DeckEventOnChairPlaced.Create(this).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnChairDestroyed.Create(this).Send();
        }

        public void SetOccupied(DeckAgentHumanoid humanoid)
        {
            IsAvailable = false;
            _humanoid = humanoid;
        }

        public void SetUnoccupied()
        {
            IsAvailable = true;
            _humanoid = null;
        }

        public void OnTableDestroyed()
        {
            if (IsAvailable)
            {
                return;
            }

            _humanoid.GetUp();
        }
    }
}