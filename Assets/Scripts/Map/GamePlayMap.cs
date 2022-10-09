using UnityEngine;

namespace Deck.Map
{
    public class GamePlayMap : MonoBehaviour
    {
        private Grid _grid;

        public void Initialize(Grid grid)
        {
            _grid = grid;
        }

        public void DrawGizmos()
        {
            for (int i = 0; i < _grid.cells.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.cells.GetLength(1); j++)
                {
                    Gizmos.DrawSphere(new Vector3(i, 0, j), 0.2f);
                }
            }
        }
    }
}