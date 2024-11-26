using Commands.Fetch;
using Deck.Base;
using Deck.InputHandling.Events;
using Deck.Services.AgentFinder;
using Deck.Services.Selection.Events;
using Deck.Utility;
using Sirenix.OdinInspector;

namespace Deck.Components.Core
{
    public class DeckAgentCore : DeckAgent
    {
        [Button]
        private void Test(DeckAgent target)
        {
            var coffeeMachines = Deck.GetService<DeckServiceFinder>().TryGetAgentsWithTag(DeckActionTag.CoffeeMachine);
            foreach (var coffeeMachine in coffeeMachines)
            {
                EnqueueCommand(new DeckCommandFetchItem(this, coffeeMachine, target));
                break;
            }
        }

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