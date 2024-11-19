using Deck.Commands;
using Deck.InputHandling.Events;
using Deck.Services;
using Deck.Utility;
using Services.AgentFinder;
using Sirenix.OdinInspector;

namespace Deck.Components.Core
{
    public class DeckAgentCore : DeckAgent
    {
        [Button]
        private void Test()
        {
            var coffeeMachines = Deck.GetService<DeckServiceFinder>().TryGetAgentsWithTag(DeckActionTag.CoffeeMachine);
            foreach (var coffeeMachine in coffeeMachines)
            {
                EnqueueCommand(new DeckCommandMove(coffeeMachine.transform.position, this));
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