using Deck.Data.Buildable;
using Deck.Services.Building;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.InGame.Agent.Building.Building.BuildingSets.BuildingWallsAndDoors
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable wallBuildable;
        [SerializeField] private DeckBuildable doorBuildable;

        private DeckEscapableBuildMode _escapableBuildMode;
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
            EscapableService = Deck.GetService<DeckServiceEscapable>();
            BuildingService = Deck.GetService<DeckServiceBuilding>();

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
                EscapableService.CloseEscapable();
            }

            _escapableBuildMode = new DeckEscapableBuildMode(OnBuildModeClosed, OnBuildRequested, true);
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

            BuildingService.BuildInCell(position);
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                BuildingService.UpdateSilhouetteInCell();
            }
        }

        private void OnBuildModeClosed()
        {
            _isBuildModeActive = false;
            _escapableBuildMode = null;
            BuildingService.Clear();
        }
    }
}