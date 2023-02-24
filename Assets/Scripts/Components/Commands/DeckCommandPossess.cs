using Cysharp.Threading.Tasks;
using Deck.Component;

namespace Deck.Components.Operations
{
    public class DeckCommandPossess : DeckCommand
    {
        private DeckAgent _agent;

        public DeckCommandPossess(DeckAgent agent)
        {
            commandType = DeckCommandType.Possess;
            _agent = agent;
        }

        public override UniTask<bool> ProcessCommand()
        {
            foreach (var deckComponent in _agent.GetDeckComponents<DeckComponent>())
            {
                deckComponent.Release();
            }

            return base.ProcessCommand();
        }
    }
}