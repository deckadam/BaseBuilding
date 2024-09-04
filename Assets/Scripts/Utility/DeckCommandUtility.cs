using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;
using UnityEngine;

namespace Deck.Utility
{
    public static class DeckCommandUtility
    {
        public static async UniTask<bool> AwaitTillDestinationIsReached(DeckComponentMovement traverser, Transform to, float desiredDistance, CancellationToken token)
        {
            var isCanceled = await UniTask.WaitWhile(() => traverser.SetDestination(to.position, desiredDistance), cancellationToken: token).SuppressCancellationThrow();
            return isCanceled;
        }
    }
}