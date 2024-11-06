using Deck.InputHandling.Events;
using Deck.Components.Tree;
using Deck.Services;
using Deck.Utility.Logger;

namespace Deck.Components.Core
{
    public class DeckAgentCore : DeckAgent
    {
        protected override void InternalRequestDestroy()
        {
            DeckEventOnCoreAgentDeath.Create(this).Send();
            Destroy(gameObject);
        }

        private void OnEnable()
        {
            DeckLogger.Level("Adding player");
            DeckEventOnCoreAgentCreated.Create(this).Send();
        }
    }
}