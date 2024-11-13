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
        private Dictionary<Vector2Int, DeckItemVisual> _activeDoors;

        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override void InternalOnInitialize()
        {
            _doorCheckSet = new HashSet<Vector2Int>();
            _activeDoors = new Dictionary<Vector2Int, DeckItemVisual>();
        }

        public void ReturnItemVisual(Vector2Int cellIndex)
        {
            if (!_doorCheckSet.Contains(cellIndex)) return;

            _doorCheckSet.Remove(cellIndex);
            ReturnIfHasItemVisual(_activeDoors[cellIndex]);
            _wallProvider.OnDoorRemoved(cellIndex);
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!itemVisual.PrefabId.Equals(doorPrefab.PrefabId)) return false;

            var pos = itemVisual.transform.position.ToVector2Int();
            _doorCheckSet.Remove(pos);
            ReturnIfHasItemVisual(itemVisual);

            _wallProvider.OnDoorRemoved(pos);
            _activeDoors.Remove(pos);
            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!prefabId.Equals(doorPrefab.PrefabId))
            {
                itemVisual = null;
                return false;
            }

            if (!_doorCheckSet.Add(cellIndex))
            {
                itemVisual = _activeDoors[cellIndex];
                return true;
            }

            itemVisual = null;
            RentIfHasItemVisual(prefabId, out itemVisual);
            _activeDoors[cellIndex] = itemVisual;
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