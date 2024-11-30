using Deck.Base;
using Deck.Commands;
using Deck.InputHandling.Events;
using Deck.Utility;
using Deck.Waiter;
using UnityEngine;

namespace Deck.InGame.Agent.Waiter
{
    public class DeckAgentWaiter : DeckAgentHumanoid
    {
        protected override void InternalHumanoidDespawnRequested()
        {
            Deck.GetService<DeckServiceWaiter>().RemoveAvailableWaiter(this);
        }

        protected override void InternalHumanoidSpawnRequested()
        {
            DeckEventOnCoreAgentCreated.Create(this).Send();
        }

        protected override void InternalOnWaiting()
        {
            Debug.LogError("Waiting");
            Deck.GetService<DeckServiceWaiter>().RegisterAvailableWaiter(this);
        }

        public void ProcessOrder(DeckCommand order)
        {
            EnqueueCommand(order);
        }
    }
}