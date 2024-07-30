using Deck.Data.Buildable;
using Deck.Services;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable wallBuildable;
        [SerializeField] private DeckBuildable doorBuildable;

        private DeckEscapableBuildMode _escapableBuildMode;
        private DeckServiceCamera _cameraService;
        private DeckServiceBuilding _buildingService;
        private DeckServiceEscapable _escapableService;
        private bool _isBuildModeActive;

        protected override void OnInitialize()
        {
            _cameraService = Deck.GetService<DeckServiceCamera>();
            _escapableService = Deck.GetService<DeckServiceEscapable>();
            _buildingService = Deck.GetService<DeckServiceBuilding>();
        }

        public void OnBuildDoorClicked()
        {
            OnBuildableSelected(doorBuildable);
        }

        public void OnBuildWallClicked()
        {
            OnBuildableSelected(wallBuildable);
        }

        private void OnBuildableSelected(DeckBuildable buildable)
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.IsEscaped())
            {
                _escapableBuildMode.OnCloseRequested();
            }

            _escapableBuildMode = new DeckEscapableBuildMode(OnBuildModeClosed, OnBuildRequested, true);
            _escapableService.RegisterEscapable(_escapableBuildMode);
            _isBuildModeActive = true;
            _buildingService.StartSilouette(buildable);
        }

        private void OnBuildRequested(Vector3 position)
        {
            if (!_isBuildModeActive)
            {
                return;
            }

            _buildingService.Build(position);
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                _buildingService.UpdateSilouette(_cameraService.GetCursorWorldPosition());
            }
        }

        private void OnBuildModeClosed()
        {
            _isBuildModeActive = false;
            _escapableBuildMode = null;
            _buildingService.Clear();
        }
    }
}