using System;
using System.Collections.Generic;
using System.Linq;
using Deck.Item;
using Deck.Save;
using Deck.UI.InGame;
using Deck.Utility;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderDoor", menuName = "Service/ItemVisualManager/DeckItemVisualProviderDoor")]
    public class DeckItemVisualProviderDoor : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisual doorPrefab;

        private readonly Quaternion HorizontalRotation = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion VerticalRotation = Quaternion.Euler(0, 0, 0);

        private Dictionary<Vector2Int, DeckItemVisual> _activeDoors;
        private HashSet<Vector2Int> _doorCheckSet;

        private DeckItemVisualProviderWall _wallProvider;

        [Inject]
        private void Inject(DeckItemVisualProviderWall wallProvider)
        {
            _wallProvider = wallProvider;
        }

        protected override void OnInitialize()
        {
            _activeDoors = new Dictionary<Vector2Int, DeckItemVisual>();
            _doorCheckSet = new HashSet<Vector2Int>();
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!itemVisual.PrefabId.Equals(doorPrefab.PrefabId)) return false;

            var pos = itemVisual.transform.position.ToVector2Int();
            _doorCheckSet.Remove(pos);
            ReturnIfHasItemVisual(itemVisual);

            _wallProvider.OnDoorRemoved(pos);
            return true;
        }

        public override bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool isInternal = true)
        {
            if (!prefabId.Equals(doorPrefab.PrefabId))
            {
                itemVisual = null;
                return false;
            }

            _doorCheckSet.Add(cellIndex);
            itemVisual = null;
            RentIfHasItemVisual(prefabId, out itemVisual, isInternal);
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
                Debug.LogError("Horizontal Rotation");
                return HorizontalRotation;
            }


            if (neighbourSet[2] || neighbourSet[3])
            {
                Debug.LogError("Vertical Rotation");
                return VerticalRotation;
            }

            return HorizontalRotation;
        }

        protected override string OnSaveDataRequested()
        {
            var saveData = new SaveData
            {
                wallPositions = _activeDoors.Keys.ToArray()
            };
            return DeckSaveUtility.GetSerializedData(saveData);
        }

        protected override void OnLoadDataRequested(string value)
        {
            var saveData = DeckSaveUtility.GetDeserializedData<SaveData>(value);
            _doorCheckSet.AddRange(saveData.wallPositions);
            foreach (var vector2Int in _doorCheckSet)
            {
                RequestItemVisual(doorPrefab.PrefabId, vector2Int, out _);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public Vector2Int[] wallPositions;
            public Direction[] directions;
        }

        private enum Direction
        {
            Vertical,
            Horizontal
        }
    }
}