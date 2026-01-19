using System;
using System.Collections.Generic;
using Base;
using Data.Buildable;
using Services;
using Services.Currency;
using Services.Finder;
using Services.ItemVisual;
using Systems.SystemSave;
using UnityEngine;
using Utility;

namespace InGame.Agent.Building
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

        public void InitializeBuildingWithVisual(DeckItemVisual itemVisual)
        {
            RaiseItemVisualChanged();

            itemVisualInstance = itemVisual;
            itemVisualInstance.transform.parent = SelfTransform;

            if (!setVisualPosition) return;

            itemVisualInstance.transform.localPosition = Vector3.zero;
            itemVisualInstance.transform.localRotation = Quaternion.identity;
        }

        public void InitializeBuilding()
        {
            if (!buildingData.IsGridBased)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance);
                RaiseItemVisualChanged();

                itemVisualInstance.transform.parent = SelfTransform;

                if (setVisualPosition)
                {
                    itemVisualInstance.transform.localPosition = Vector3.zero;
                    itemVisualInstance.transform.localRotation = Quaternion.identity;
                }
            }
            else if (buildingData.Indices.Length == 1)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance, SelfTransform.position.ToVector2Int());
                RaiseItemVisualChanged();

                itemVisualInstance.transform.parent = SelfTransform;

                if (setVisualPosition)
                {
                    itemVisualInstance.transform.localPosition = Vector3.zero;
                    itemVisualInstance.transform.localRotation = Quaternion.identity;
                }
            }
            // else if (buildingData.Indices.Length > 1)
            // {
            // DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestMultipleItemVisual(this, buildingData.ItemVisual.PrefabId, buildingData.Indices.Length, buildingData.Indices, out itemVisualInstances);
            // RaiseItemVisualChanged();
            // SetItemVisuals(itemVisualInstances);

            // for (var i = 0; i < itemVisualInstances.Length; i++)
            // {
            // var deckItemVisual = itemVisualInstances[i];
            // deckItemVisual.transform.parent = SelfTransform;

            // if (!setVisualPosition) continue;

            // deckItemVisual.transform.localPosition = buildingData.Indices[i].ToVector3();
            // deckItemVisual.transform.localRotation = Quaternion.identity;
            // }
            // }

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

            if (isMultiInstance && itemVisualInstances != null)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().ReturnItemVisuals(itemVisualInstances);
            }
            else if (itemVisualInstance != null)
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().ReturnItemVisual(itemVisualInstance);
            }

            DeckServiceProvider.GetService<DeckServiceCurrency>().ChangeValueRelative(buildingData.Prices, true);

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
                var buildingOnTop = (DeckAgentBuilding)DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(i);
                AddBuildingToTop(buildingOnTop);
            }

            InternalLoadAdditionalBuildingData(additionalData.internalAdditionalData);
        }

        public void SetBuildingData(DeckBuildable dataToSet)
        {
            buildingData = dataToSet;
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