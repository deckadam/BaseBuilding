using System.Threading;
using Base;
using Cysharp.Threading.Tasks;

namespace Commands
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
            foreach (var deckComponent in _target.Components)
            {
                deckComponent.Release();
            }

            return default;
        }
    }
}