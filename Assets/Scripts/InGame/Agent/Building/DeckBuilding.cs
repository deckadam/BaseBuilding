using Deck.Agent;
using Deck.Data.Buildable;
using Deck.Item;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Deck.UI
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private DeckItemVisual itemVisualPrefab;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private bool setVisualRotation;

        private DeckItemVisual _itemVisualInstance;

        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(itemVisualPrefab.PrefabId, out _itemVisualInstance, selfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(selfTransform.position, buildingData.Indices, this);

            if (_itemVisualInstance != null)
            {
                _itemVisualInstance.transform.SetParent(transform, false);

                if (setVisualPosition)
                {
                    _itemVisualInstance.transform.localPosition = Vector3.zero;
                }

                if (setVisualRotation)
                {
                    _itemVisualInstance.transform.localRotation = Quaternion.identity;
                }
            }
        }

        protected override void InternalRequestDeath()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            if (_itemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(_itemVisualInstance);
            }
        }

        public DeckBuildable BuildingData => buildingData;
    }
}