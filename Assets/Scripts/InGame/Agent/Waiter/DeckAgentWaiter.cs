using Deck.Base;
using Deck.InputHandling.Events;
using Deck.Utility;
using Deck.Waiter;

namespace Deck.InGame.Agent.Waiter
{
    public class DeckAgentWaiter : DeckAgentHumanoid
    {
        public override void OnSpawned()
        {
            Deck.GetService<DeckServiceWaiter>().RegisterAvailableWaiter(this);
        }

        public override void OnDespawned()
        {
            Deck.GetService<DeckServiceWaiter>().RemoveAvailableWaiter(this);
        }

        protected override void AfterInitializationCompleted()
        {
            DeckEventOnCoreAgentCreated.Create(this).Send();
        }
    }
}