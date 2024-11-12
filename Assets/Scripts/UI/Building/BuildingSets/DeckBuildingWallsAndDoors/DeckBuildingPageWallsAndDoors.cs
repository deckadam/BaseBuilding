using Deck.Data.Buildable;
using Deck.Services.Building;
using Deck.Services.CameraService;
using Deck.UI.Building.BuildingSets.BuildMode;
using Deck.Utility;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets.BuildingWallsAndDoors
{
    public class DeckBuildingPageWallsAndDoors : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable wallBuildable;
        [SerializeField] private DeckBuildable doorBuildable;

        private DeckBuildableButton _wallButton;
        private DeckBuildableButton _doorButton;
        private DeckBuildable _selectedBuildable;
        private bool _isBuildingWall;

        private readonly Quaternion _horizontalRotation = Quaternion.Euler(0, 90, 0);
        private readonly Quaternion _verticalRotation = Quaternion.Euler(0, 0, 0);

        protected override void InternalOnInitialize()
        {
            escapableService = Deck.GetService<DeckServiceEscapable>();
            buildingService = Deck.GetService<DeckServiceBuilding>();

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

            if (escapableBuildMode != null)
            {
                escapableService.RemoveEscapable(escapableBuildMode);
            }

            if (_selectedBuildable == wallBuildable)
            {
                escapableBuildMode = new DeckEscapableBuildModeWall(OnEscapeRequested, true);
                _isBuildingWall = true;
                buildingService.StartSilhouetteRect(buildable);
            }
            else
            {
                escapableBuildMode = new DeckEscapableBuildMode(OnEscapeRequested, OnBuildRequested, true);
                _isBuildingWall = false;
                buildingService.StartSilhouette(buildable);
            }

            escapableService.RegisterEscapable(escapableBuildMode);
            isBuildModeActive = true;
        }

        private void OnBuildRequested(Vector3[] positions)
        {
            if (!isBuildModeActive)
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
                buildingService.BuildInCell(position.ToVector2Int());
            }
        }

        private void Update()
        {
            if (isBuildModeActive)
            {
                if (!_isBuildingWall)
                {
                    var cellIndex = Deck.GetService<DeckServiceCamera>().GetCursorCellIndex();
                    var neighbourStatus = buildingService.GetCellNeighbourStatus(cellIndex);

                    if (neighbourStatus[2] && neighbourStatus[3])
                    {
                        buildingService.UpdateSilhouetteInCell(_verticalRotation);
                    }
                    else
                    {
                        buildingService.UpdateSilhouetteInCell(_horizontalRotation);
                    }
                }
            }
        }
    }
}