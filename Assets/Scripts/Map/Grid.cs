using System;
using UnityEngine;

namespace Deck.Map
{
    [Serializable]
    public class Grid
    {
        public Cell[,] cells { get; private set; }

        public Grid(int width, int height)
        {
            cells = new Cell[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var newCell = new Cell(GetCellPosition(i, j));

                    cells[i, j] = newCell;
                }
            }
        }

        private Vector3 GetCellPosition(int width, int height)
        {
            return new Vector3(width, 0, height);
        }
    }
}