using System.Collections.Generic;
using Deck.Utility;
using UnityEngine;
using Utility;

namespace Deck.InGame.Area
{
    public class DeckInGameArea
    {
        private HashSet<Vector2Int> _cellPositions;
        private HashSet<Vector2Int> _wallPositions;

        public DeckInGameArea(HashSet<Vector2Int> cellPositions, HashSet<Vector2Int> wallPositions)
        {
            _cellPositions = cellPositions;
            _wallPositions = wallPositions;
        }

        public void DrawGizmo()
        {
            foreach (var pos in _wallPositions)
            {
                Gizmos.DrawSphere(pos.ToVector3() + Vector3.up * 3f, 0.2f);
            }

            foreach (var pos in _cellPositions)
            {
                Gizmos.DrawCube(pos.ToVector3() + Vector3.up * 3f, Vector3.one / 2f);
            }
        }

        public HashSet<Vector2Int> GetCellPositions()
        {
            return _cellPositions;
        }

        public HashSet<Vector2Int> GetWallPositions()
        {
            return _wallPositions;
        }
    }
}