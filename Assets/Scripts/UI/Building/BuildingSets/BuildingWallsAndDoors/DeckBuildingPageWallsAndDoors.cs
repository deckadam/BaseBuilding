using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Utility;
using Deck.Utility.Logger;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.Components.Building.Building.BuildingSets.BuildingWallsAndDoors
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable wallBuildable;
        [SerializeField] private DeckBuildable doorBuildable;

        private DeckEscapableBuildMode _escapableBuildMode;
        private DeckBuildableButton _wallButton;
        private DeckBuildableButton _doorButton;
        private DeckBuildable _selectedBuildable;
        private bool _isBuildModeActive;

        private void OnDestroy()
        {
            if (_escapableBuildMode != null && !_escapableBuildMode.HasEscaped)
            {
                _escapableBuildMode.OnCloseRequested();
            }
        }

        protected override void OnInitialize()
        {
            EscapableService = Deck.GetService<DeckServiceEscapable>();
            BuildingService = Deck.GetService<DeckServiceBuilding>();

            _wallButton = InstanceProvider.RentUIElement<DeckBuildableButton>();
            _wallButton.Initialize(OnBuildableSelected, wallBuildable);
            _wallButton.rectTransform.SetParent(container, false);

            _doorButton = InstanceProvider.RentUIElement<DeckBuildableButton>();
            _doorButton.Initialize(OnBuildableSelected, doorBuildable);
            _doorButton.rectTransform.SetParent(container, false);
        }

        private void OnBuildableSelected(DeckBuildable buildable)
        {
            _selectedBuildable = buildable;

            if (_escapableBuildMode != null && !_escapableBuildMode.HasEscaped)
            {
                EscapableService.CloseEscapable();
            }

            if (_selectedBuildable == wallBuildable)
            {
                _escapableBuildMode = new DeckEscapableBuildModeWall(OnBuildModeClosed, OnBuildRequested, true);
            }
            else
            {
                _escapableBuildMode = new DeckEscapableBuildMode(OnBuildModeClosed, OnBuildRequested, true);
            }

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

            if (_selectedBuildable == wallBuildable)
            {
                BuildingService.BuildInCellRect(positions);
            }
            else
            {
                if (positions.Length > 1)
                {
                    DeckLogger.Error("OnBuildRequested: multiple positions not are supported for door");
                }

                BuildingService.BuildInCell(positions[0].ToVector2Int());
            }
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
            if (!_escapableBuildMode.HasEscaped)
            {
                Deck.GetService<DeckServiceEscapable>().ListEscapables();
                _escapableBuildMode.OnCloseRequested();
                _escapableBuildMode = null;
            }

            BuildingService.Clear();
        }
    }
}