using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Commands;
using Deck.Components;

namespace Commands.Fetch
{
    public class DeckCommandFetchItem : DeckCommand
    {
        private DeckAgent _fetcher;
        private DeckAgent _fetchTarget;
        private DeckAgent _deliverTarget;

        public DeckCommandFetchItem(DeckAgent fetcher, DeckAgent fetchTarget, DeckAgent deliverTarget)
        {
            _fetcher = fetcher;
            _fetchTarget = fetchTarget;
            _deliverTarget = deliverTarget;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_fetchTarget.transform.position, _fetcher).ProcessCommand(token);
            await UniTask.Delay(1000, cancellationToken: token).SuppressCancellationThrow();
            await new DeckCommandMove(_deliverTarget.transform.position, _fetcher).ProcessCommand(token);

            return true;
        }
    }
}