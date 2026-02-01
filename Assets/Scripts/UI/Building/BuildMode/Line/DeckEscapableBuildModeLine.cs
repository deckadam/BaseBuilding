using Services;
using Services.Camera;
using UnityEngine;

namespace UI.Building.BuildMode.Line
{
    public class DeckEscapableBuildModeLine : DeckEscapableBuildModePartialCancel
    {
        private Vector2Int _initialCellPosition;
        private Vector2Int _currentCellPosition;

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

            var delta = currentCellPosition - _initialCellPosition;

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                currentCellPosition.y = _initialCellPosition.y;
            }
            else
            {
                currentCellPosition.x = _initialCellPosition.x;
            }

            BuildingService.BuildInRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void InternalOnMouseMove()
        {
            var currentCellPosition = DeckServiceProvider.GetService<DeckServiceCamera>().GetCursorCellIndex();

            if (_currentCellPosition == currentCellPosition)
            {
                return;
            }

            _currentCellPosition = currentCellPosition;

            if (_isDown)
            {
                var delta = currentCellPosition - _initialCellPosition;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                {
                    currentCellPosition.y = _initialCellPosition.y;
                }
                else
                {
                    currentCellPosition.x = _initialCellPosition.x;
                }

                _currentCellPosition = currentCellPosition;
                BuildingService.UpdateSilhouetteInCellRect(new[] { _initialCellPosition, currentCellPosition });
            }
            else
            {
                BuildingService.UpdateSilhouetteInCellRect(new[] { currentCellPosition, currentCellPosition });
            }
        }

        protected override void InternalOnMiddleScroll()
        {
            
        }

        public override bool ShouldEscapeFullyOnRightClick => false;
    }
}