using System;
using System.Collections.Generic;
using Deck.Base;
using Deck.Data.Buildable;
using Deck.Instancing;
using Deck.Save;
using Deck.Services.AgentFinder;
using Deck.Services.Building;
using Deck.Services.Currency;
using Deck.Services.ItemVisual;
using Deck.Utility;
using UI.Building.BuildMode;
using UnityEngine;
using Zenject;

namespace Deck.Components.Building
{
    public class DeckBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private List<DeckBuilding> buildingsOnTop = new();
        [SerializeField] private DeckBuilding onTopOf;

        public DeckBuildable BuildingData => buildingData;

        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        protected override void AfterLoad()
        {
            InitializeBuilding();

            switch (buildingData.BuildMode)
            {
                case DeckBuildMode.Rect or DeckBuildMode.InCell or DeckBuildMode.Line:
                {
                    break;
                }
                case DeckBuildMode.BuildOnTopWithAccessArea:
                {
                    var buildingService = Deck.GetService<DeckServiceBuilding>();
                    buildingService.SetCellOccupied(transform.position.ToVector2Int(), buildingData.AccessIndices, this);
                    break;
                }
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

            InternalAfterBuildingInitialized();
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

            if (itemVisualInstance != null)
            {
                Deck.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualInstance);
            }

            Deck.GetService<DeckServiceCurrency>().ChangeValueRelative(buildingData.Prices, true);

            _instanceProvider.ReturnAgent(this);
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
                buildingsOnTopIds[index] = deckBuilding.UniqueId.ID;
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

        protected virtual void InternalAfterBuildingInitialized()
        {
        }
    }

    [Serializable]
    public class LayerData
    {
        public int[] agentUniqueIdsOfBuildingsOnTop;
    }
}