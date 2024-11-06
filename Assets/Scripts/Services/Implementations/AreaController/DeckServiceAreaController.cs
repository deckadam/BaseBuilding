using System.Collections.Generic;
using Deck.EventManager;
using Deck.Services;
using Deck.Utility;
using Deck.InGame.Area;
using Services.Implementations.AreaController.Events;
using UnityEngine;

namespace Services.Implementations.AreaController
{
    public class DeckServiceAreaController : DeckServiceBase
    {
        private List<DeckInGameArea> _areas;

        public override void Initialize()
        {
            _areas = new List<DeckInGameArea>();
        }

        public override void AfterGameSessionInitialized()
        {
            DeckEventManager.Register<DeckEventOnAreaBuild>(OnAreaBuild);
        }

        public override void BeforeGameSessionDeinitialized()
        {
            DeckEventManager.Unregister<DeckEventOnAreaBuild>(OnAreaBuild);
        }

        private void OnAreaBuild(DeckEventOnAreaBuild obj)
        {
            var first = obj.positions[0].ToVector2Int();
            var second = obj.positions[1].ToVector2Int();
            if (!IsViableArea(first, second))
            {
                return;
            }

            var newAreaGameObject = new GameObject();
            var newArea = newAreaGameObject.AddComponent<DeckInGameArea>();
            newArea.name = "ClosedArea";
            newArea.InitializeArea(first, second, obj.cellPositions);
            _areas.Add(newArea);
        }

        private bool IsViableArea(Vector2Int first, Vector2Int second)
        {
            var xDiff = Mathf.Abs(first.x - second.x);
            var yDiff = Mathf.Abs(first.y - second.y);

            if (xDiff <= 1)
            {
                return false;
            }

            if (yDiff <= 1)
            {
                return false;
            }

            return true;
        }

        public void RemoveArea(DeckInGameArea area)
        {
            _areas.Remove(area);
        }

        protected override void DrawGizmos()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            foreach (var deckArea in _areas)
            {
                deckArea.DrawGizmo();
            }
        }
    }
}