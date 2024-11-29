using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using UnityEngine;

namespace Deck.Commands.Fetch
{
    public class DeckCommandFetchItem : DeckCommand
    {
        private DeckAgent _fetcher;
        private Vector3 _fetchPosition;
        private DeckAgent _deliverTarget;

        public DeckCommandFetchItem(DeckAgent fetcher, Vector3 fetchPosition, DeckAgent deliverTarget)
        {
            _fetcher = fetcher;
            _fetchPosition = fetchPosition;
            _deliverTarget = deliverTarget;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_fetchPosition, _fetcher).ProcessCommand(token);
            await UniTask.Delay(1000, cancellationToken: token).SuppressCancellationThrow();
            Debug.LogError("Fetch second part");
            await new DeckCommandMove(_deliverTarget.transform.position, _fetcher).ProcessCommand(token);
            Debug.LogError("Fetch completed");
            return true;
        }
    }
}