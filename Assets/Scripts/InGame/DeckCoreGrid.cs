using UnityEngine;

namespace Deck.Map
{
    public class DeckCoreGrid
    {
        public DeckCell[,] cells { get; }
        public Vector2Int size { get; }

        public DeckCoreGrid(Vector2Int size)
        {
            this.size = size;
            cells = new DeckCell[size.x, size.y];
            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    var newCell = new DeckCell(GetCellPosition(i, j), new Vector2Int(i, j));

                    cells[i, j] = newCell;
                }
            }
        }

        private Vector3 GetCellPosition(int width, int height)
        {
            return new(width - size.x / 2f, 0, height - size.y / 2f);
        }
    }
}