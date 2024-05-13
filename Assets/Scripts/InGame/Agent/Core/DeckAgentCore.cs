using Deck.Services;
using Deck.InputHandling.Events;
using Deck.Utility.Logger;

namespace Deck.Agent
{
    public class DeckAgentCore : DeckAgent
    {
        protected override void InternalRequestDeath()
        {
            DeckOnCoreAgentDeathEvent.Create(this).Send();
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            DeckLogger.Level("Adding player");
            DeckOnCoreAgentCreatedEvent.Create(this).Send();
        }
    }
}