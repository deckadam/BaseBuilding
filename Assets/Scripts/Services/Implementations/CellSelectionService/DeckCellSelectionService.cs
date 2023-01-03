using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Map;
using Deck.Utility;
using UnityEngine;
using Utility;
namespace Deck.Services.Implementations.CellSelectionService
{
    public class DeckCellSelectionService : DeckServiceBase
    {
        private DeckCell _currentDeckCell;
        private Texture2D _selectionTexture;
        private Material _groundMaterial;
        private Vector2Int _size;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckOnCellClicked>(OnCellClicked);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnCellClicked>(OnCellClicked);
        }

        public void SetMapData(Vector2Int size, Material groundMaterial)
        {
            _size = size;
            _selectionTexture = DeckUtility.GenerateTexture(size);
            _groundMaterial = groundMaterial;
            _groundMaterial.SetTexture(DeckShaderConstants.SelectionTexture, _selectionTexture);
        }

        private void OnCellClicked(DeckOnCellClicked obj)
        {
            _currentDeckCell = obj.deckCell;
            _selectionTexture.SetPixel(_size.x - obj.deckCell.cellIndex.x, _size.y - obj.deckCell.cellIndex.y, Color.red);
            _selectionTexture.Apply();
        }
    }
}