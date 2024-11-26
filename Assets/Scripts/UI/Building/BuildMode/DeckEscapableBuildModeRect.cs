using Deck.InputHandling.Events;
using Deck.Services.Building;
using Deck.Services.Cam;
using UnityEngine;

namespace Deck.UI.Building.BuildMode
{
    public class DeckEscapableBuildModeRect : DeckEscapableBuildMode
    {
        private Vector2Int _initialCellPosition;
        private Vector2Int _currentCellPosition;

        private bool _isDown;

        protected override void InternalOnInitialize()
        {
            BuildingService.StartSilhouetteRect(Buildable);
        }

        protected override void InternalOnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
            _isDown = true;
            _initialCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
        }

        protected override void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            if (!_isDown)
            {
                return;
            }

            _isDown = false;
            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
            Deck.GetService<DeckServiceBuilding>().BuildInRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            if (!_isDown)
            {
                return;
            }

            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();

            if (_currentCellPosition != currentCellPosition)
            {
                _currentCellPosition = currentCellPosition;
                return;
            }

            _currentCellPosition = currentCellPosition;
            BuildingService.UpdateSilhouetteInCellRect(new[] { _initialCellPosition, currentCellPosition });
        }
    }
}