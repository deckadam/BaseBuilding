using Cysharp.Threading.Tasks;
using Deck.Agent;

namespace Deck.Components
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

            return default;
        }
    }
}