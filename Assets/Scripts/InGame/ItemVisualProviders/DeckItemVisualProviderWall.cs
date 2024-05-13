using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.Item;
using Deck.Utility;
using Deck.Utility.Iterators;
using UnityEngine;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderWall", menuName = "Service/ItemVisualManager/DeckItemVisualProviderWall")]
    public class DeckItemVisualProviderWall : DeckItemVisualProviderBasic
    {
        [SerializeField] private WallItemVisualParts[] wallItemVisualParts;
        [SerializeField] private int wallSizeMultiplier = 3;
        private Dictionary<(Vector2Int, Vector2Int), DeckItemVisual> _activeConnectorVisuals;
        private Dictionary<Guid, Dictionary<Vector2Int, bool>> _activeItemVisuals;

        protected override void OnInitialize()
        {
            _activeItemVisuals = new Dictionary<Guid, Dictionary<Vector2Int, bool>>();
            _activeConnectorVisuals = new Dictionary<(Vector2Int, Vector2Int), DeckItemVisual>();
            foreach (var wallItemVisualPart in wallItemVisualParts)
            {
                _activeItemVisuals.Add(wallItemVisualPart.wallMainPart.UniqueId.ID, new Dictionary<Vector2Int, bool>());
            }
        }

        protected override async void OnSpawned(DeckItemVisual itemVisual)
        {
            WallItemVisualParts? part = null;
            foreach (var t in wallItemVisualParts)
            {
                if (t.wallMainPart.UniqueId.ID != itemVisual.UniqueId.ID)
                    continue;

                // Debug.LogError(t.wallMainPart.UniqueId.ID + "    " + itemVisual.UniqueId.ID);
                part = t;
                break;
            }

            if (!part.HasValue)
                return;

            await UniTask.NextFrame();

            var pos = itemVisual.transform.position.ToVector2Int();
            var checkSet = _activeItemVisuals[itemVisual.UniqueId.ID];
            checkSet[pos] = true;
            PlaceConnectors(part.Value, checkSet, pos);
        }

        protected override void OnDespawned(DeckItemVisual itemVisual)
        {
            WallItemVisualParts? part = null;
            foreach (var t in wallItemVisualParts)
            {
                if (t.wallMainPart.UniqueId.ID != itemVisual.UniqueId.ID) 
                    continue;
                part = t;
                break;
            }

            if (!part.HasValue)
                return;

            var pos = itemVisual.transform.position.ToVector2Int();
            var checkSet = _activeItemVisuals[itemVisual.UniqueId.ID];
            checkSet[pos] = false;
            RemoveConnectors(pos);
        }

        private void PlaceConnectors(WallItemVisualParts part, IReadOnlyDictionary<Vector2Int, bool> checkSet, Vector2Int targetPos)
        {
            foreach (var neighbourPos in DeckIterator.GetNeighbourIterator(targetPos, wallSizeMultiplier))
            {
                if (checkSet.ContainsKey(neighbourPos) && checkSet[neighbourPos])
                {
                    PlaceConnector(part, targetPos, neighbourPos);
                }
            }
        }

        private void RemoveConnectors(Vector2Int targetPos)
        {
            foreach (var neighbourPos in DeckIterator.GetNeighbourIterator(targetPos, wallSizeMultiplier))
            {
                if (_activeConnectorVisuals.ContainsKey((targetPos, neighbourPos)))
                {
                    ReturnIfHasItemVisual(_activeConnectorVisuals[(targetPos, neighbourPos)]);
                    _activeConnectorVisuals.Remove((targetPos, neighbourPos));
                }
                else if (_activeConnectorVisuals.ContainsKey((neighbourPos, targetPos)))
                {
                    ReturnIfHasItemVisual(_activeConnectorVisuals[(neighbourPos, targetPos)]);
                    _activeConnectorVisuals.Remove((neighbourPos, targetPos));
                }
            }
        }

        private void PlaceConnector(WallItemVisualParts part, Vector2Int targetPos, Vector2Int neighbourPos)
        {
            RentIfHasItemVisual(part.wallConnectorPart.UniqueId, out var itemVisual);
            itemVisual.transform.position = DeckVectorUtility.GetPlaneMiddlePosition(targetPos, neighbourPos);
            itemVisual.transform.rotation = DeckVectorUtility.GetPlaneRotation(targetPos, neighbourPos);
            _activeConnectorVisuals.Add((targetPos, neighbourPos), itemVisual);
        }

        [Serializable]
        private struct WallItemVisualParts
        {
            public DeckItemVisual wallMainPart;
            public DeckItemVisual wallConnectorPart;
        }
    }
}