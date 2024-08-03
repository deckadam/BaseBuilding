using System;
using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.DeckBuildingPageFurniture
{
    public class DeckBuildingPageFurniture : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable[] furnitureBuildable;
        [SerializeField] private RectTransform container;

        private DeckServiceBuilding _buildingService;
        private DeckServiceEscapable _escapableService;

        private DeckEscapableBuildMode _escapableBuildMode;
        private bool _isBuildModeActive;

        protected override void OnInitialize()
        {
            _buildingService = Deck.GetService<DeckServiceBuilding>();
            _escapableService = Deck.GetService<DeckServiceEscapable>();

            foreach (var buildable in furnitureBuildable)
            {
                var buildableButton = uiPool.Rent<DeckBuildableButton>();
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
                _escapableBuildMode.OnCloseRequested();
            }

            _escapableBuildMode = new DeckEscapableBuildMode(OnEscapeRequested, OnBuildRequested);
            _escapableService.RegisterEscapable(_escapableBuildMode);
            _isBuildModeActive = true;
            _buildingService.StartSilouette(buildable, true);
        }

        private void OnBuildRequested(Vector3 position)
        {
            if (!_isBuildModeActive)
            {
                return;
            }

            _buildingService.BuildFree(position, _buildingService.GetRotation());
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                _buildingService.UpdateSilouette();
            }
        }

        private void OnEscapeRequested()
        {
            _isBuildModeActive = false;
            _escapableBuildMode = null;
            _buildingService.Clear();
        }
    }
}