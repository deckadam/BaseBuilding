using System;
using Deck.InputHandling.Events;
using Deck.Services.Building;
using Deck.Services.CameraService;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.BuildMode
{
    public class DeckEscapableBuildModeWall : DeckEscapableBuildMode
    {
        private Vector2Int _initialCellPosition;
        private Vector2Int _currentCellPosition;

        public DeckEscapableBuildModeWall(Action onEscape, bool canMoveBuild = false) : base(onEscape, null, canMoveBuild)
        {
        }

        protected override void OnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
            _initialCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
        }

        protected override void OnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
            Deck.GetService<DeckServiceBuilding>().BuildInRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void OnMouseMove(DeckEventOnMouseMove obj)
        {
            var currentCellPositions = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();

            if (_currentCellPosition != currentCellPositions)
            {
                _currentCellPosition = currentCellPositions;
                return;
            }

            _currentCellPosition = currentCellPositions;
            BuildingService.UpdateSilhouetteInCellRect(new[] { _initialCellPosition, currentCellPositions });
        }
    }
}