using System.Threading;
using Cysharp.Threading.Tasks;
using Deck;
using Deck.Events.CellSelectionService;
using UnityEngine;

namespace Deck.Components
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
            _agent.GetDeckComponent<DeckComponentMovement>().SetDestination(_targetPosition, 0f);
            return default;
        }
    }
}