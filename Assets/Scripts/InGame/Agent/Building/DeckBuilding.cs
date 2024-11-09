using Deck.Data.Buildable;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Services.Implementations.Currency;
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
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(buildingData.ItemVisual.PrefabId, out ItemVisualInstance, selfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(selfTransform.position.ToVector2Int(), buildingData.Indices, this);

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

        protected sealed override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            if (ItemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(ItemVisualInstance);
            }

            Deck.GetService<DeckServiceCurrency>().ChangeValueRelative(buildingData.Prices, true);
            OnBuildingDestroyed();
        }

        protected virtual void OnBuildingDestroyed()
        {
        }

        public DeckBuildable BuildingData => buildingData;

        public void SetBuildingData(DeckBuildable buildingData)
        {
            this.buildingData = buildingData;
        }
    }
}