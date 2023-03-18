using UnityEngine;

namespace Deck.Map
{
    public class DeckCell
    {
        public Vector3 position { get; }
        public DeckAgentBlockade resident { get; private set; }
        public Vector2Int cellIndex { get; }

        public DeckCell(Vector3 position, Vector2Int cellIndex)
        {
            this.position = position;
            this.cellIndex = cellIndex;
        }

        public void SetResident(DeckAgentBlockade newResident)
        {
            resident = newResident;
        }
    }
}