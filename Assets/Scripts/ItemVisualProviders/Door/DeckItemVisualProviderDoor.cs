using System.Collections.Generic;
using Base;
using ItemVisualProviders.Door.Events;
using UnityEngine;
using Utility;
using Zenject;

namespace ItemVisualProviders.Door
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderDoor", menuName = "Service/ItemVisualManager/DeckItemVisualProviderDoor")]
    public class DeckItemVisualProviderDoor : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisual doorPrefab;

        private readonly Quaternion _horizontalRotation = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion _verticalRotation = Quaternion.Euler(0, 0, 0);

        private HashSet<Vector2Int> _doorCheckSet;
        private Dictionary<Vector2Int, DeckItemVisual> _activeDoors;

        protected override void InternalOnInitialize()
        {
            _doorCheckSet = new HashSet<Vector2Int>();
            _activeDoors = new Dictionary<Vector2Int, DeckItemVisual>();
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!itemVisual.PrefabId.Equals(doorPrefab.PrefabId)) return false;

            var pos = itemVisual.transform.position.ToVector2Int();
            if (!_doorCheckSet.Contains(pos))
            {
                DeckLogger.Inform("Non registered door tried to return item visual");
                return false;
            }

            _doorCheckSet.Remove(pos);
            TryReturnItemVisual(itemVisual);

            DeckEventOnDoorRemoved.Create(pos).Send();
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

            DeckEventOnDoorPlaced.Create(cellIndex).Send();
            return true;
        }

        private Quaternion GetDoorRotation(Vector2Int cellIndex)
        {

            var neighbourSet = new bool[4];
            DeckEventOnNeighbourSetRequested.Create(cellIndex, ref neighbourSet).Send();

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