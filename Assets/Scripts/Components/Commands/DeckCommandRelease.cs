using System.Threading;
using Cysharp.Threading.Tasks;
using Deck;

namespace Deck.Components
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
            foreach (var deckComponent in _target.GetDeckComponents<DeckComponent>())
            {
                deckComponent.Release();
            }

            return default;
        }
    }
}