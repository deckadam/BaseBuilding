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

        public bool ReturnItemVisual(Vector2Int cellIndex)
        {
            if (!_doorCheckSet.Contains(cellIndex)) return false;

            _doorCheckSet.Remove(cellIndex);
            ReturnIfHasItemVisual(_activeWalls[cellIndex]);
            _wallProvider.OnDoorRemoved(cellIndex);
            return true;
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

        public override bool RequestItemVisual(DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual, bool isInternal = true)
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

            if (_wallProvider.IsWall(cellIndex))
            {
                _wallProvider.ReturnItemVisual(cellIndex);
            }


            _doorCheckSet.Add(cellIndex);
            itemVisual = null;
            RentIfHasItemVisual(prefabId, out itemVisual, isInternal);
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
                return HorizontalRotation;
            }

            if (neighbourSet[2] || neighbourSet[3])
            {
                return VerticalRotation;
            }

            return HorizontalRotation;
        }

        protected override string OnSaveDataRequested()
        {
            return string.Empty;
            var saveData = new SaveData
            {
                wallPositions = _doorCheckSet.ToArray()
            };

            return DeckSaveUtility.GetSerializedData(saveData);
        }

        protected override void OnLoadDataRequested(string value)
        {
            return;
            var saveData = DeckSaveUtility.GetDeserializedData<SaveData>(value);
            _doorCheckSet.AddRange(saveData.wallPositions);
            foreach (var pos in _doorCheckSet)
            {
                RequestItemVisual(doorPrefab.PrefabId, pos, out _);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public Vector2Int[] wallPositions;
        }
    }
}