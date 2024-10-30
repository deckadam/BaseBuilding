using Deck.Data.Buildable;
using Deck.Components;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Deck.Components.Building
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private bool setVisualRotation;


        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(buildingData.ItemVisual.PrefabId, out itemVisualPrefab, selfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(selfTransform.position, buildingData.Indices, this);

            if (itemVisualPrefab != null)
            {
                itemVisualPrefab.transform.SetParent(transform, false);
                itemVisualPrefab.Agent = this;
                if (setVisualPosition)
                {
                    itemVisualPrefab.transform.localPosition = Vector3.zero;
                }

                if (setVisualRotation)
                {
                    itemVisualPrefab.transform.localRotation = Quaternion.identity;
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
            if (itemVisualPrefab != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualPrefab);
            }
        }

        public DeckBuildable BuildingData => buildingData;

        public void SetBuildingData(DeckBuildable buildingData)
        {
            this.buildingData = buildingData;
        }
    }
}