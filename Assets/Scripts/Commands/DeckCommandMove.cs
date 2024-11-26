using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;
using UnityEngine;

namespace Deck.Commands
{
    public class DeckCommandMove : DeckCommand
    {
        private Vector3 _targetPosition;
        private DeckAgent _agent;

        public DeckCommandMove(Vector3 targetPosition, DeckAgent agent)
        {
            _targetPosition = targetPosition;
            _agent = agent;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var componentMovement = _agent.GetDeckComponent<DeckComponentMovement>();
            componentMovement.SetDestination(_targetPosition);
            await UniTask.NextFrame(token);
            await UniTask.WaitUntil(() => componentMovement.ReachedToDestination(), cancellationToken: token);
            return true;
        }
    }
}