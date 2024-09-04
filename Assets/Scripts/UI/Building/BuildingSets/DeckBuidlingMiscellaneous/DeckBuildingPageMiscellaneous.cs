using Deck.Data.Buildable;
using Deck.Services.Building;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.InGame.Agent.Building.Building.BuildingSets.BuildingMiscellaneous
{
    public class DeckBuildingPageMiscellaneous : DeckBuildingPage
    {
        [SerializeField] private DeckBuildable[] miscellaneousBuildable;

        private DeckEscapableBuildMode _escapableBuildMode;
        private DeckBuildable _selectedMiscellaneous;
        private bool _isBuildModeActive;

        protected override void OnInitialize()
        {
            BuildingService = Deck.GetService<DeckServiceBuilding>();
            EscapableService = Deck.GetService<DeckServiceEscapable>();

            foreach (var buildable in miscellaneousBuildable)
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
                EscapableService.CloseEscapable();
            }

            _selectedMiscellaneous = buildable;
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

            if (_selectedMiscellaneous.CanBeHangedToWall)
            {
                BuildingService.BuildOnWall();
            }
            else if (_selectedMiscellaneous.CanBePlacedOnTopOfAnotherObject)
            {
                BuildingService.BuildOnTop();
            }
            else
            {
                BuildingService.BuildFree(position);
            }
        }

        private void Update()
        {
            if (!_isBuildModeActive) return;

            if (_selectedMiscellaneous.CanBeHangedToWall)
            {
                BuildingService.UpdateSilhouetteOnWall();
            }
            else if (_selectedMiscellaneous.CanBePlacedOnTopOfAnotherObject)
            {
                BuildingService.UpdateSilhouetteOnTop();
            }
            else
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