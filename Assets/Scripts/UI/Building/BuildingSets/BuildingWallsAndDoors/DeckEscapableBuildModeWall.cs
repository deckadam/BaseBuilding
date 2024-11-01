using System;
using Deck.InputHandling.Events;
using Deck.Services.Building;
using Deck.Services.CameraService;
using UnityEngine;

namespace Deck.Components.Building.Building.BuildingSets.BuildingWallsAndDoors
{
    public class DeckEscapableBuildModeWall : DeckEscapableBuildMode
    {
        public DeckEscapableBuildModeWall(Action onEscape, Action<Vector3[]> onBuild, bool canMoveBuild = false) : base(onEscape, onBuild, canMoveBuild)
        {
        }

        private Vector3 _initialCellPosition;

        protected override void OnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
            Debug.LogError("Mouse down");
            _initialCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition();
        }

        protected override void OnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            Debug.LogError("Mouse up");
            var currentCellPosition = Deck.GetService<DeckServiceCamera>().GetCursorWorldPosition();
            Deck.GetService<DeckServiceBuilding>().BuildInCellRect(new[] { _initialCellPosition, currentCellPosition });
        }

        protected override void OnMouseMove(DeckEventOnMouseMove obj)
        {
        }
    }
}