using Deck.Data.Buildable;
using Deck.Services.Building;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.Components.Building.Building.BuildingSets.DeckBuildingFurniture
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
            if (_escapableBuildMode != null && !_escapableBuildMode.IsEscaped())
            {
                _escapableBuildMode.OnCloseRequested();
            }
        }

        private void OnBuildableClicked(DeckBuildable buildable)
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.IsEscaped())
            {
                EscapableService.CloseEscapable();
            }


            _escapableBuildMode = new DeckEscapableBuildMode(OnEscapeRequested, OnBuildRequested);
            EscapableService.RegisterEscapable(_escapableBuildMode);
            _isBuildModeActive = true;
            BuildingService.StartSilhouette(buildable);
        }

        private void OnBuildRequested(Vector3 position)
        {
            if (!_isBuildModeActive)
            {
                return;
            }

            BuildingService.BuildFree(position);
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
            _escapableBuildMode = null;
            BuildingService.Clear();
        }
    }
}