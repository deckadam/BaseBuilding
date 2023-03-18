using Deck.Data.Map;
using Deck.Map;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Services.Implementations.GridService
{
    public class DeckGridService : DeckServiceBase
    {
        private DeckBinderMap _binderMap;
        private DeckCoreGrid _activeGrid;
        private Vector2Int _activeGridSize;

        [Inject]
        private void Inject(DeckBinderMap binderMap)
        {
            _binderMap = binderMap;
        }

        public void GenerateGrid(out DeckCoreGrid deckCoreGrid)
        {
            DeckLogger.Grid("Generating grid");
            deckCoreGrid = new DeckCoreGrid(_binderMap.size);

            _activeGridSize = _binderMap.size;
            _activeGrid = deckCoreGrid;

            deckCoreGrid = _activeGrid;
        }

        public DeckCell GetCellWithWorldPosition(Vector3 worldPosition)
        {
            if (_activeGrid == null) return null;
            var x = (int)Mathf.Ceil(worldPosition.x) + _activeGridSize.x / 2;
            var y = (int)Mathf.Ceil(worldPosition.z) + _activeGridSize.y / 2;

            return _activeGrid.cells[x, y];
        }
    }
}