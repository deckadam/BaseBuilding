using Deck.Components.Operations;
using UnityEngine;

namespace Deck.Components
{
    public class DeckCommandMove : DeckCommand
    {
        public Vector3 targetPosition;

        public DeckCommandMove(Vector3 targetPosition)
        {
            commandType = DeckCommandType.Move;
            this.targetPosition = targetPosition;
        }
    }
}