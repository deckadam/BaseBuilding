using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Services.CellSelectionService;
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

        public override UniTask<bool> ProcessCommand(CancellationToken token)
        {
            DeckServiceSelection.ResetSelectionToPossession();
            var componentMovement = _agent.GetDeckComponent<DeckComponentMovement>();
            componentMovement.SetDestination(_targetPosition);
            return default;
        }
    }
}