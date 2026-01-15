using System.Collections.Generic;
using System.Linq;
using Deck.InGame.Area;
using EventManager;
using Services;
using Services.AreaController.Events;
using UnityEngine;

namespace Deck.Services.AreaController
{
    public class DeckServiceAreaController : DeckServiceBase
    {
        private List<DeckInGameArea> _areas;
        private HashSet<Vector2Int> _areaPositions;

        public override void Initialize()
        {
            _areas = new List<DeckInGameArea>();
            _areaPositions = new HashSet<Vector2Int>();
        }

        public override void WarmUp()
        {
            DeckEventManager.Register<DeckEventOnWallBuild>(OnWallBuild);
            DeckEventManager.Register<DeckEventOnWallDestroyed>(OnWallDestroyed);
            DeckEventManager.Register<DeckEventOnDoorBuild>(OnDoorBuild);
            DeckEventManager.Register<DeckEventOnDoorDestroyed>(OnDoorDestroyed);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnWallBuild>(OnWallBuild);
            DeckEventManager.Unregister<DeckEventOnWallDestroyed>(OnWallDestroyed);
            DeckEventManager.Unregister<DeckEventOnDoorBuild>(OnDoorBuild);
            DeckEventManager.Unregister<DeckEventOnDoorDestroyed>(OnDoorDestroyed);
        }

        private void OnDoorBuild(DeckEventOnDoorBuild obj)
        {
            _areaPositions.Add(obj.position);
            RecalculateAreas();
        }

        private void OnDoorDestroyed(DeckEventOnDoorDestroyed obj)
        {
            _areaPositions.Remove(obj.position);
            RecalculateAreas();
        }

        private void OnWallBuild(DeckEventOnWallBuild obj)
        {
            _areaPositions.Add(obj.position);
            RecalculateAreas();
        }

        private void OnWallDestroyed(DeckEventOnWallDestroyed obj)
        {
            _areaPositions.Remove(obj.position);
            RecalculateAreas();
        }

        private void RecalculateAreas()
        {
            _areas.Clear();

            if (_areaPositions.Count == 0)
            {
                return;
            }

            _areas = GetClosedAreasFromPoints(_areaPositions);
        }

        protected override void DrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (_areas == null)
            {
                return;
            }

            foreach (var inGameArea in _areas)
            {
                inGameArea.DrawGizmo();
            }
        }

        private bool IsBetweenFourWalls(Vector2Int point, HashSet<Vector2Int> points)
        {
            //Up
            if (!points.Any(item => item.x == point.x && item.y > point.y))
            {
                return false;
            }

            //Down
            if (!points.Any(item => item.x == point.x && item.y < point.y))
            {
                return false;
            }

            //Right
            if (!points.Any(item => item.x < point.x && item.y == point.y))
            {
                return false;
            }

            //Left
            if (!points.Any(item => item.x > point.x && item.y == point.y))
            {
                return false;
            }

            return true;
        }

        private void FloodFillRecursive(Vector2Int current, HashSet<Vector2Int> grid, HashSet<Vector2Int> visited, HashSet<Vector2Int> nonVisited, HashSet<Vector2Int> encounteredWalls, HashSet<Vector2Int> filledArea, int minX, int maxX, int minY, int maxY)
        {
            if (current.x < minX || current.x > maxX || current.y < minY || current.y > maxY)
            {
                return;
            }

            var isWall = false;
            if (grid.Contains(current))
            {
                isWall = true;
                encounteredWalls.Add(current);
            }

            if (!visited.Add(current))
            {
                return;
            }

            if (!isWall && IsBetweenFourWalls(current, grid))
            {
                filledArea.Add(current);
            }
            else
            {
                return;
            }

            nonVisited.Remove(current);

            FloodFillRecursive(new Vector2Int(current.x + 1, current.y), grid, visited, nonVisited, encounteredWalls, filledArea, minX, maxX, minY, maxY); // Right
            FloodFillRecursive(new Vector2Int(current.x - 1, current.y), grid, visited, nonVisited, encounteredWalls, filledArea, minX, maxX, minY, maxY); // Left
            FloodFillRecursive(new Vector2Int(current.x, current.y + 1), grid, visited, nonVisited, encounteredWalls, filledArea, minX, maxX, minY, maxY); // Up
            FloodFillRecursive(new Vector2Int(current.x, current.y - 1), grid, visited, nonVisited, encounteredWalls, filledArea, minX, maxX, minY, maxY); // Down
        }

        //TODO: Optimize this later v0.3
        private List<DeckInGameArea> GetClosedAreasFromPoints(HashSet<Vector2Int> grid)
        {
            var minX = grid.Min(item => item.x);
            var maxX = grid.Max(item => item.x);
            var minY = grid.Min(item => item.y);
            var maxY = grid.Max(item => item.y);

            var visited = new HashSet<Vector2Int>();
            var filledArea = new HashSet<Vector2Int>();
            var encounteredWalls = new HashSet<Vector2Int>();

            var nonVisited = new HashSet<Vector2Int>();
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    nonVisited.Add(new Vector2Int(x, y));
                }
            }

            var endResult = new List<DeckInGameArea>();

            while (nonVisited.Count > 0)
            {
                var current = nonVisited.First();
                nonVisited.Remove(current);
                FloodFillRecursive(current, grid, visited, nonVisited, encounteredWalls, filledArea, minX, maxX, minY, maxY);
                if (filledArea.Count > 0)
                {
                    endResult.Add(new DeckInGameArea(new HashSet<Vector2Int>(filledArea), new HashSet<Vector2Int>(encounteredWalls)));
                    filledArea.Clear();
                    encounteredWalls.Clear();
                }
            }

            return endResult;
        }
    }
}