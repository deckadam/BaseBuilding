using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Utility;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.DeckBuildingFurniture
{
    public class DeckBuildingPageFurniture : DeckBuildingPage
    {
        private DeckEscapableBuildMode _escapableBuildMode;
        private bool _isBuildModeActive;

        protected override void OnInitialize()
        {
            BuildingService = Deck.GetService<DeckServiceBuilding>();
            EscapableService = Deck.GetService<DeckServiceEscapable>();

            foreach (var buildable in buildables)
            {
                var buildableButton = InstanceProvider.RentUIElement<DeckBuildableButton>();
                buildableButton.Initialize(OnBuildableClicked, buildable);
                buildableButton.rectTransform.SetParent(container, false);
            }
        }

        private void OnDestroy()
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.HasEscaped)
            {
                _escapableBuildMode.OnCloseRequested();
            }
        }

        private void OnBuildableClicked(DeckBuildable buildable)
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.HasEscaped)
            {
                EscapableService.CloseEscapable();
            }


            _escapableBuildMode = new DeckEscapableBuildMode(OnEscapeRequested, OnBuildRequested);
            EscapableService.RegisterEscapable(_escapableBuildMode);
            _isBuildModeActive = true;
            BuildingService.StartSilhouette(buildable);
        }

        private void OnBuildRequested(Vector3[] positions)
        {
            if (!_isBuildModeActive)
            {
                return;
            }

            if (positions.Length > 1)
            {
                DeckLogger.Error("OnBuildRequested: multiple positions not are supported for furniture");
            }

            BuildingService.BuildFree(positions[0]);
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                BuildingService.UpdateSilhouetteFree();
            }
        }

        private void OnEscapeRequested()
        {
            _isBuildModeActive = false;

            if (!_escapableBuildMode.HasEscaped)
            {
                EscapableService.CloseEscapable();
            }

            _escapableBuildMode = null;
            BuildingService.Clear();
        }
    }
}