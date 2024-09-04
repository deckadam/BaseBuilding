using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.InGame.Agent;

namespace Deck.Commands
{
    public class DeckCommandRelease : DeckCommand
    {
        private DeckAgent _target;

        public DeckCommandRelease(DeckAgent target)
        {
            _target = target;
        }

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            foreach (var deckComponent in _target.GetDeckComponents())
            {
                deckComponent.Release();
            }

            return default;
        }
    }
}