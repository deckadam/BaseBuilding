using Deck.Services.Cam;
using Services.Building;
using Systems.SystemInput.Events;
using UnityEngine;

namespace UI.Building.BuildMode
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
            _initialCellPosition = Services.DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();
        }

        protected override void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            if (!_isDown)
            {
                return;
            }

            _isDown = false;
            var currentCellPosition = Services.DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();
            Services.DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            if (!_isDown)
            {
                return;
            }

            var currentCellPosition = Services.DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();

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