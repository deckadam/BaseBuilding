using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.UI.Building.BuildingSets.BuildMode;
using Deck.Utility;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.DeckBuildingFurniture
{
    public class DeckBuildingPageFurniture : DeckBuildingPage
    {
        protected override void InternalOnInitialize()
        {
            buildingService = Deck.GetService<DeckServiceBuilding>();
            escapableService = Deck.GetService<DeckServiceEscapable>();

            foreach (var buildable in buildables)
            {
                var buildableButton = InstanceProvider.RentUIElement<DeckBuildableButton>();
                buildableButton.Initialize(OnBuildableClicked, buildable);
                buildableButton.rectTransform.SetParent(container, false);
            }
        }

        private void OnBuildableClicked(DeckBuildable buildable)
        {
            if (escapableBuildMode != null)
            {
                escapableService.RemoveEscapable(escapableBuildMode);
            }

            escapableBuildMode = new DeckEscapableBuildMode(OnEscapeRequested, OnBuildRequested);
            escapableService.RegisterEscapable(escapableBuildMode);
            isBuildModeActive = true;
            buildingService.StartSilhouette(buildable);
        }

        private void OnBuildRequested(Vector3[] positions)
        {
            if (!isBuildModeActive)
            {
                return;
            }

            if (positions.Length > 1)
            {
                DeckLogger.Error("OnBuildRequested: multiple positions not are supported for furniture");
            }

            buildingService.BuildFree(positions[0]);
        }

        private void Update()
        {
            if (isBuildModeActive)
            {
                buildingService.UpdateSilhouetteFree();
            }
        }
    }
}