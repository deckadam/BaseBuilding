using Deck.Data.Buildable;
using Deck.Services.Building;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable wallBuildable;
        [SerializeField] private DeckBuildable doorBuildable;

        [SerializeField] private RectTransform container;

        private DeckEscapableBuildMode _escapableBuildMode;
        private DeckServiceBuilding _buildingService;
        private DeckServiceEscapable _escapableService;
        private DeckBuildableButton _wallButton;
        private DeckBuildableButton _doorButton;
        private bool _isBuildModeActive;

        private void OnDestroy()
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.IsEscaped())
            {
                _escapableBuildMode.OnCloseRequested();
            }
        }

        protected override void OnInitialize()
        {
            _escapableService = Deck.GetService<DeckServiceEscapable>();
            _buildingService = Deck.GetService<DeckServiceBuilding>();

            _wallButton = uiPool.Rent<DeckBuildableButton>();
            _wallButton.Initialize(OnBuildableSelected, wallBuildable);
            _wallButton.rectTransform.SetParent(container, false);

            _doorButton = uiPool.Rent<DeckBuildableButton>();
            _doorButton.Initialize(OnBuildableSelected, doorBuildable);
            _doorButton.rectTransform.SetParent(container, false);
        }

        private void OnBuildableSelected(DeckBuildable buildable)
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.IsEscaped())
            {
                _escapableService.CloseEscapable();
            }

            _escapableBuildMode = new DeckEscapableBuildMode(OnBuildModeClosed, OnBuildRequested, true);
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

            _buildingService.BuildInCell(position);
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                _buildingService.UpdateSilouette();
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