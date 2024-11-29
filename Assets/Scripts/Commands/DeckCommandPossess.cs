using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;

namespace Deck.Commands
{
    public class DeckCommandPossess : DeckCommand
    {
        private DeckAgent _agent;

        public DeckCommandPossess(DeckAgent agent)
        {
            _agent = agent;
        }

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            foreach (var deckComponent in _agent.Components)
            {
                deckComponent.Release();
            }

            return default;
        }
    }
}