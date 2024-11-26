using Deck.InputHandling.Events;
using Deck.Services.Cam;
using UnityEngine;

namespace Deck.UI.Building.BuildMode
{
    public class DeckEscapableBuildModeLine : DeckEscapableBuildMode
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

        protected override void InternalOnMouseMove(DeckEventOnMouseMove obj)
        {
            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();

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
    }
}