using Services;
using Services.Building;
using Services.Camera;
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

        protected override void InternalOnLeftClickDown()
        {
            _isDown = true;
            _initialCellPosition = DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();
        }

        protected override void InternalOnLeftClickUp()
        {
            if (!_isDown)
            {
                return;
            }

            _isDown = false;
            var currentCellPosition = DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void InternalOnMouseMove()
        {
            if (!_isDown)
            {
                return;
            }

            var currentCellPosition = DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();

            if (_currentCellPosition != currentCellPosition)
            {
                _currentCellPosition = currentCellPosition;
                return;
            }

            _currentCellPosition = currentCellPosition;
            BuildingService.UpdateSilhouetteInCellRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void InternalOnBuildModeCanceled()
        {
            BuildingService.ClearAll();
        }
    }
}