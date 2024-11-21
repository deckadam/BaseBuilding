using System;
using System.Collections.Generic;
using Deck.Data.Buildable;
using Deck.ItemVisualProviders;
using Deck.Save;
using Deck.Services.Building;
using Deck.Services.Implementations.Currency;
using Deck.Utility;
using Services.AgentFinder;
using UI.Building.BuildMode;
using UnityEngine;

namespace Deck.Components.Building
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private List<DeckBuilding> buildingsOnTop = new();
        [SerializeField] private DeckBuilding onTopOf;

        public DeckBuildable BuildingData => buildingData;

        public override void AfterLoad()
        {
            InitializeBuilding();

            if (buildingData.IsCellBased)
            {
                var buildingService = Deck.GetService<DeckServiceBuilding>();
                buildingService.SetCellOccupied(transform.position.ToVector2Int(), buildingData.Indices, this);
                buildingService.SetCellOccupied(transform.position.ToVector2Int(), buildingData.AccessIndices, this);
            }

            if (buildingData.BuildMode == DeckBuildMode.ItemWithAccessArea)
            {
                var buildingService = Deck.GetService<DeckServiceBuilding>();
                buildingService.SetCellOccupied(transform.position.ToVector2Int(), buildingData.AccessIndices, this);
            }
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
            foreach (var deckBuilding in buildingsOnTop)
            {
                deckBuilding.RequestDestroy();
            }

            buildingsOnTop.Clear();

            if (onTopOf != null)
            {
                onTopOf.RemoveBuildingFromTop(this);
            }

            Deck.GetService<DeckServiceBuilding>().OnBuildingDestroyed(this);
            if (itemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualInstance);
            }

            Deck.GetService<DeckServiceCurrency>().ChangeValueRelative(buildingData.Prices, true);
        }

        public void AddBuildingToTop(DeckBuilding building)
        {
            buildingsOnTop.Add(building);
            building.onTopOf = this;
        }

        private void RemoveBuildingFromTop(DeckBuilding building)
        {
            buildingsOnTop.Remove(building);
            building.onTopOf = null;
        }

        public override string GetAdditionalData()
        {
            var buildingsOnTopIds = new int[buildingsOnTop.Count];

            for (var index = 0; index < buildingsOnTop.Count; index++)
            {
                var deckBuilding = buildingsOnTop[index];
                buildingsOnTopIds[index] = deckBuilding.GetUniqueId().ID;
            }

            var layerData = new LayerData()
            {
                agentUniqueIdsOfBuildingsOnTop = buildingsOnTopIds
            };

            return DeckSaveUtility.GetSerializedData(layerData);
        }

        protected override void LoadAdditionalData(string data)
        {
            var layerData = DeckSaveUtility.GetDeserializedData<LayerData>(data);
            foreach (var i in layerData.agentUniqueIdsOfBuildingsOnTop)
            {
                var buildingOnTop = (DeckBuilding)Deck.GetService<DeckServiceFinder>().GetAgent(i);
                AddBuildingToTop(buildingOnTop);
            }
        }

        public void SetBuildingData(DeckBuildable buildingData)
        {
            this.buildingData = buildingData;
        }
    }

    [Serializable]
    public class LayerData
    {
        public int[] agentUniqueIdsOfBuildingsOnTop;
    }
}