using System.Collections.Generic;
using Deck.EventManager;
using Deck.Utility;
using Services.Implementations.AreaController;
using Services.Implementations.AreaController.Events;
using UnityEngine;

namespace Deck.InGame.Area
{
    public class DeckInGameArea : MonoBehaviour
    {
        private List<Vector2Int> _positions;
        private Vector2Int _firstPosition;
        private Vector2Int _secondPosition;

        private List<Vector2Int> _doorPositions;

        public void InitializeArea(Vector2Int firstPosition, Vector2Int secondPosition, List<Vector2Int> objCellPositions)
        {
            _doorPositions = new List<Vector2Int>();

            _firstPosition = firstPosition;
            _secondPosition = secondPosition;
            _positions = objCellPositions;
        }

        private void Awake()
        {
            DeckEventManager.Register<DeckEventOnDoorBuild>(OnDoorBuild);
            DeckEventManager.Register<DeckEventOnWallDestroyed>(OnWallDestroyed);
        }

        private void OnDestroy()
        {
            DeckEventManager.Unregister<DeckEventOnDoorBuild>(OnDoorBuild);
            DeckEventManager.Unregister<DeckEventOnWallDestroyed>(OnWallDestroyed);
        }


        private void OnWallDestroyed(DeckEventOnWallDestroyed obj)
        {
            if (!_positions.Contains(obj.position))
            {
                return;
            }
            
            Deck.GetService<DeckServiceAreaController>().RemoveArea(this);
            Destroy(gameObject);
        }

        private void OnDoorBuild(DeckEventOnDoorBuild obj)
        {
            if (!_positions.Contains(obj.position))
            {
                return;
            }

            _doorPositions.Add(obj.position);
        }

        public void DrawGizmo()
        {
            var minX = Mathf.Min(_firstPosition.x, _secondPosition.x);
            var maxX = Mathf.Max(_firstPosition.x, _secondPosition.x);

            var minY = Mathf.Min(_firstPosition.y, _secondPosition.y);
            var maxY = Mathf.Max(_firstPosition.y, _secondPosition.y);

            for (int x = minX + 1; x < maxX; x++)
            {
                for (int y = minY + 1; y < maxY; y++)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawSphere(new Vector3(x, 0.2f, y), 0.25f);
                }
            }

            foreach (var doorPosition in _doorPositions)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(doorPosition.ToVector3(), doorPosition.ToVector3() + Vector3.up * 5);
            }
        }
    }
}