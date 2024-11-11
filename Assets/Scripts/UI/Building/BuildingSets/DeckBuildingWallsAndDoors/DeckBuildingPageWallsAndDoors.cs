using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Deck.UI.Building.BuildingWallsAndDoors;
using Deck.Utility;
using Deck.Utility.Logger;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.BuildingWallsAndDoors
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
        private bool _isBuildingWall;

        private readonly Quaternion _horizontalRotation = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion _verticalRotation = Quaternion.Euler(0, 0, 0);

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
                _escapableBuildMode = new DeckEscapableBuildModeWall(OnBuildModeClosed, true);
                _isBuildingWall = true;
                BuildingService.StartSilhouetteRect(buildable);
            }
            else
            {
                _escapableBuildMode = new DeckEscapableBuildMode(OnBuildModeClosed, OnBuildRequested, true);
                _isBuildingWall = false;
                BuildingService.StartSilhouette(buildable);
            }

            EscapableService.RegisterEscapable(_escapableBuildMode);
            _isBuildModeActive = true;
        }

        private void OnBuildRequested(Vector3[] positions)
        {
            if (!_isBuildModeActive)
            {
                return;
            }

            if (_selectedBuildable != wallBuildable)
            {
                if (positions.Length > 1)
                {
                    DeckLogger.Error("OnBuildRequested: multiple positions not are supported for door");
                }

                var position = positions[0];
                BuildingService.BuildInCell(position.ToVector2Int());
            }
        }

        private void Update()
        {
            if (_isBuildModeActive)
            {
                if (!_isBuildingWall)
                {
                    var cellIndex = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
                    var neighbourStatus = BuildingService.GetCellNeighbourStatus(cellIndex);

                    if (neighbourStatus[2] && neighbourStatus[3])
                    {
                        BuildingService.UpdateSilhouetteInCell(cellIndex, _verticalRotation);
                    }
                    else
                    {
                        BuildingService.UpdateSilhouetteInCell(cellIndex, _horizontalRotation);
                    }
                }
            }
        }

        private void OnBuildModeClosed()
        {
            _isBuildModeActive = false;
            if (!_escapableBuildMode.HasEscaped)
            {
                _escapableBuildMode.OnCloseRequested();
                _escapableBuildMode = null;
            }

            BuildingService.Clear();
        }
    }
}