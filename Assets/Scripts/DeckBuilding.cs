using Deck.Agent;
using Deck.Data.Buildable;
using Deck.Item;
using Deck.ItemVisualProviders;
using Deck.Services.Building;
using Deck.Utility;
using UnityEngine;

namespace Deck.UI
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private DeckItemVisual itemVisualPrefab;

        private DeckItemVisual _itemVisualInstance;

        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            _itemVisualInstance = Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(itemVisualPrefab.PrefabId,selfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();
            Deck.GetService<DeckServiceBuilding>().SetCellOccupied(selfTransform.position, buildingData.Indices, this);
        }

        protected override void InternalRequestDeath()
        {
            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(_itemVisualInstance);
        }

        public DeckBuildable BuildingData => buildingData;
    }
}