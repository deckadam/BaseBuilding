using System;
using Deck.InputHandling.Events;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Deck.Utility;
using UnityEngine;

namespace Deck.Components.Building.Building.BuildingSets.BuildingWallsAndDoors
{
    public class DeckEscapableBuildModeWall : DeckEscapableBuildMode
    {
        private Vector3 _initialCellPosition;
        private Vector3 _currentCellPosition;

        public DeckEscapableBuildModeWall(Action onEscape, Action<Vector3[]> onBuild, bool canMoveBuild = false) : base(onEscape, onBuild, canMoveBuild)
        {
        }

        protected override void OnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
            _initialCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition();
        }

        protected override void OnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition();
            Deck.GetService<DeckServiceBuilding>().BuildInCellRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void OnMouseMove(DeckEventOnMouseMove obj)
        {
            var currentCellPositions = Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition();

            if (_currentCellPosition.ToVector2Int() != currentCellPositions.ToVector2Int())
            {
                _currentCellPosition = currentCellPositions;
                return;
            }

            _currentCellPosition = currentCellPositions;
            BuildingService.UpdateSilhouetteInCellRect(new[] { _initialCellPosition, currentCellPositions });
        }
    }
}