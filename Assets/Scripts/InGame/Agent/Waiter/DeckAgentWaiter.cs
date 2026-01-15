using Deck.Base;
using Deck.Commands;
using Services.Waiter;
using Systems.SystemInput.Events;
using Utility;

namespace InGame.Agent.Waiter
{
    public class DeckAgentWaiter : DeckAgentHumanoid
    {
        protected override void InternalHumanoidDespawnRequested()
        {
            Services.DeckServiceProvider.GetService<DeckServiceWaiter>().RemoveAvailableWaiter(this);
        }

        protected override void InternalHumanoidSpawnRequested()
        {
            DeckEventOnCoreAgentCreated.Create(this).Send();
        }

        protected override void InternalOnWaiting()
        {
            Services.DeckServiceProvider.GetService<DeckServiceWaiter>().RegisterAvailableWaiter(this);
        }

        public void ProcessOrder(DeckCommand order)
        {
            EnqueueCommand(order);
        }
    }
}