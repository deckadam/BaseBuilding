using Deck.Data.Buildable;
using Deck.Item;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Deck.InGame.Agent.Building
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private DeckItemVisual itemVisualPrefab;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private bool setVisualRotation;

        protected DeckItemVisual ItemVisualInstance;

        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(itemVisualPrefab.PrefabId, out ItemVisualInstance, selfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(selfTransform.position, buildingData.Indices, this);

            if (ItemVisualInstance != null)
            {
                ItemVisualInstance.transform.SetParent(transform, false);
                ItemVisualInstance.Agent = this;
                if (setVisualPosition)
                {
                    ItemVisualInstance.transform.localPosition = Vector3.zero;
                }

                if (setVisualRotation)
                {
                    ItemVisualInstance.transform.localRotation = Quaternion.identity;
                }
            }
            else
            {
                DeckLogger.Warning("No ItemVisualInstance assigned");
            }
        }

        protected override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            if (ItemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(ItemVisualInstance);
            }
        }

        public DeckBuildable BuildingData => buildingData;
    }
}