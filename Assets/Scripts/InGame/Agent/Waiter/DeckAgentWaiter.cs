using Deck.Base;
using Deck.InputHandling.Events;
using Deck.Services.Selection.Events;
using Deck.Utility;
using Deck.Waiter;

namespace Deck.InGame.Agent.Waiter
{
    public class DeckAgentWaiter : DeckAgent
    {
        protected override void InternalRequestDestroy()
        {
            DeckEventOnCoreAgentDeath.Create(this).Send();
            Destroy(gameObject);
        }

        protected override void AfterInitializationCompleted()
        {
            DeckEventOnCoreAgentCreated.Create(this).Send();
            Deck.GetService<DeckServiceWaiter>().RegisterAvailableWaiter(this);
        }
    }
}