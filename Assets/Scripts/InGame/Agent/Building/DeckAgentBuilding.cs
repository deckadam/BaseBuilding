using System;
using System.Collections.Generic;
using Deck.Base;
using Deck.Data.Buildable;
using Deck.Save;
using Deck.Services.Currency;
using Deck.Services.Finder;
using Deck.Services.ItemVisual;
using Deck.Utility;
using UnityEngine;

namespace Deck.InGame.Agent.Building
{
    public class DeckAgentBuilding : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        [SerializeField] private bool setVisualPosition;
        [SerializeField] private List<DeckAgentBuilding> buildingsOnTop = new();
        [SerializeField] private DeckAgentBuilding onTopOf;

        public DeckBuildable BuildingData => buildingData;

        protected override void AfterLoad()
        {
            InitializeBuilding();
        }

        public void InitializeBuilding()
        {
            Deck.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance, SelfTransform.position.ToVector2Int());
            RaiseItemVisualChanged();

            itemVisualInstance.transform.parent = SelfTransform;

            if (setVisualPosition)
            {
                itemVisualInstance.transform.localPosition = Vector3.zero;
                itemVisualInstance.transform.localRotation = Quaternion.identity;
            }

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

            instanceProvider.ReturnAgent(this);
        }

        public void AddBuildingToTop(DeckAgentBuilding agentBuilding)
        {
            buildingsOnTop.Add(agentBuilding);
            agentBuilding.onTopOf = this;
        }

        private void RemoveBuildingFromTop(DeckAgentBuilding agentBuilding)
        {
            buildingsOnTop.Remove(agentBuilding);
            agentBuilding.onTopOf = null;
        }

        public sealed override string GetAdditionalData()
        {
            var buildingsOnTopIds = new int[buildingsOnTop.Count];

            for (var index = 0; index < buildingsOnTop.Count; index++)
            {
                var deckBuilding = buildingsOnTop[index];
                buildingsOnTopIds[index] = deckBuilding.UniqueId.ID;
            }

            var layerData = new BuildingAdditionalData()
            {
                agentUniqueIdsOfBuildingsOnTop = buildingsOnTopIds,
                internalAdditionalData = InternalGetAdditionalBuildingData()
            };

            return DeckSaveUtility.GetSerializedData(layerData);
        }

        protected sealed override void LoadAdditionalData(string data)
        {
            var additionalData = DeckSaveUtility.GetDeserializedData<BuildingAdditionalData>(data);
            foreach (var i in additionalData.agentUniqueIdsOfBuildingsOnTop)
            {
                var buildingOnTop = (DeckAgentBuilding)Deck.GetService<DeckServiceFinder>().GetAgent(i);
                AddBuildingToTop(buildingOnTop);
            }

            InternalLoadAdditionalBuildingData(additionalData.internalAdditionalData);
        }

        public void SetBuildingData(DeckBuildable buildingData)
        {
            this.buildingData = buildingData;
        }

        public virtual Vector3[] GetAccessPosition()
        {
            throw new Exception("Not implemented");
        }

        protected virtual void InternalAfterBuildingInitialized()
        {
        }

        protected virtual string InternalGetAdditionalBuildingData()
        {
            return string.Empty;
        }

        protected virtual void InternalLoadAdditionalBuildingData(string data)
        {
        }

        [Serializable]
        private class BuildingAdditionalData
        {
            public int[] agentUniqueIdsOfBuildingsOnTop;
            public string internalAdditionalData;
        }
    }
}