using Cysharp.Threading.Tasks;
using Deck.Component;
using Deck.Components.Operations;
using UnityEngine;

namespace Deck.Components
{
    public class DeckCommandMove : DeckCommand
    {
        private Vector3 _targetPosition;
        private DeckAgent _agent;

        public DeckCommandMove(Vector3 targetPosition, DeckAgent agent)
        {
            commandType = DeckCommandType.Movement;
            _targetPosition = targetPosition;
            _agent = agent;
        }

        public override UniTask<bool> ProcessCommand()
        {
            _agent.GetDeckComponent<DeckMovementComponent>().SetDestination(_targetPosition);
            return base.ProcessCommand();
        }
    }
}