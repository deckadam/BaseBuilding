using UnityEngine;

namespace Deck.Map
{
    public class Cell
    {
        public Vector3 position { get; }
        public CellResident resident { get; private set; }

        public Cell(Vector3 position)
        {
            this.position = position;
        }

        public void SetResident(CellResident newResident)
        {
            resident = newResident;
        }
    }
}