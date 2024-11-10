using Deck.Data.Buildable;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Services.Implementations.Currency;
using Deck.Utility;
using UnityEngine;

namespace Deck.Components.Building
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private bool setVisualPosition;

        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance, SelfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            
            itemVisualInstance.transform.parent = SelfTransform;

            if (!setVisualPosition) return;
            
            itemVisualInstance.transform.localPosition = Vector3.zero;
            itemVisualInstance.transform.localRotation = Quaternion.identity;
        }

        protected sealed override void InternalRequestDestroy()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            if (itemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualInstance);
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