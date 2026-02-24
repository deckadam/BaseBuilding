using System.Threading;
using Components.Movement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Utility
{
    public static class DeckCommandUtility
    {
        public static async UniTask<bool> AwaitTillDestinationIsReached(DeckComponentMovement componentMovement, Transform to, float desiredDistance, CancellationToken token)
        {
            var isCanceled = await UniTask.WaitWhile(() => componentMovement.SetDestination(to.position, desiredDistance: desiredDistance), cancellationToken: token).SuppressCancellationThrow();
            return isCanceled;
        }
    }
}