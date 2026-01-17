using System.Threading;
using Components.Movement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility
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