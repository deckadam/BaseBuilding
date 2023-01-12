using System;
using Deck.InputHandling.Events;
using Deck.Player;
using Deck.Services;
using Deck.Services.Implementations.GridService;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.InputHandling
{
    public class DeckInputHandler : MonoBehaviour
    {
        public static Action<bool> OnShiftStatusChange;
        public static Action OnMouseDrag;


        private DeckGridService _deckGridService;

        private Plane _basePlane;

        private void OnEnable()
        {
            _deckGridService = DeckServiceLocator.GetService<DeckGridService>();
            _basePlane = new Plane(Vector3.up, 0);
        }

        private void Update()
        {
            CheckForNavMeshHit();
            CheckForCoreAgentSelection(out var result);

            if (!result)
            {
                CheckForCellSelection();
            }

            RaycastToGround();
            CheckForShiftClick();
        }

        private void CheckForShiftClick()
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                OnShiftStatusChange?.Invoke(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
            {
                OnShiftStatusChange?.Invoke(false);
            }
        }

        private void CheckForCoreAgentSelection(out bool result)
        {
            result = false;
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 7))
            {
                return;
            }

            var agent = hit.transform.GetComponentInParent<DeckCoreAgent>();
            DeckOnCoreAgentSelected.Create(agent).Send();
            result = true;
        }

        private void CheckForCellSelection()
        {
            if (!Input.GetMouseButtonDown(0)) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _basePlane.Raycast(ray, out var dist);
            var pos = ray.GetPoint(dist);
            var cell = _deckGridService.GetCellWithWorldPosition(pos);
            if (cell == null) return;
            DeckOnCellClicked.Create(cell).Send();
        }

        private void OnDrawGizmos()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _basePlane.Raycast(ray, out var dist);
            var pos = ray.GetPoint(dist);
        }

        private void CheckForNavMeshHit()
        {
            if (!Input.GetMouseButton(1)) return;
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var positionOnGroundPlane = ray.origin - ray.direction / ray.direction.y * ray.origin.y; //collide with plane at y=0
            if (!NavMesh.SamplePosition(positionOnGroundPlane, out var navMeshHit, 1, 1)) return;

            DeckOnNavMeshPositionSelection.Create(navMeshHit.position).Send();
        }

        private void RaycastToGround()
        {
            var screenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out var hit, 100f, 1 << 6)) return;
            var pos = hit.textureCoord;
            DeckOnGroundPositionChange.Create(pos).Send();
        }
    }
}