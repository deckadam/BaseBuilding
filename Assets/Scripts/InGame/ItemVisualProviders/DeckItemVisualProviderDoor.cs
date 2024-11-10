using System.Collections.Generic;
using Deck.Base.Id;
using Deck.Components;
using Deck.Utility;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderDoor", menuName = "Service/ItemVisualManager/DeckItemVisualProviderDoor")]
    public class DeckItemVisualProviderDoor : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisual doorPrefab;

        private readonly Quaternion _horizontalRotation = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion _verticalRotation = Quaternion.Euler(0, 0, 0);

        private HashSet<Vector2Int> _doorCheckSet;
        private Dictionary<Vector2Int, DeckItemVisual> _activeWalls;

        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override void OnInitialize()
        {
            _doorCheckSet = new HashSet<Vector2Int>();
            _activeWalls = new Dictionary<Vector2Int, DeckItemVisual>();
        }

        public void ReturnItemVisual(Vector2Int cellIndex)
        {
            if (!_doorCheckSet.Contains(cellIndex)) return;

            _doorCheckSet.Remove(cellIndex);
            ReturnIfHasItemVisual(_activeWalls[cellIndex]);
            _wallProvider.OnDoorRemoved(cellIndex);
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!itemVisual.PrefabId.Equals(doorPrefab.PrefabId)) return false;

            var pos = itemVisual.transform.position.ToVector2Int();
            _doorCheckSet.Remove(pos);
            ReturnIfHasItemVisual(itemVisual);

            _wallProvider.OnDoorRemoved(pos);
            _activeWalls.Remove(pos);
            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!prefabId.Equals(doorPrefab.PrefabId))
            {
                itemVisual = null;
                return false;
            }

            if (_doorCheckSet.Contains(cellIndex))
            {
                itemVisual = null;
                return true;
            }

            _doorCheckSet.Add(cellIndex);
            itemVisual = null;
            RentIfHasItemVisual(prefabId, out itemVisual);
            _activeWalls[cellIndex] = itemVisual;
            itemVisual.transform.rotation = GetDoorRotation(cellIndex);
            itemVisual.transform.position = cellIndex.ToVector3();

            _wallProvider.OnDoorPlaced(cellIndex);
            return true;
        }

        private Quaternion GetDoorRotation(Vector2Int cellIndex)
        {
            var neighbourSet = _wallProvider.GetNeighbourSet(cellIndex);

            if (neighbourSet[0] || neighbourSet[1])
            {
                return _horizontalRotation;
            }

            if (neighbourSet[2] || neighbourSet[3])
            {
                return _verticalRotation;
            }

            return _horizontalRotation;
        }
    }
}