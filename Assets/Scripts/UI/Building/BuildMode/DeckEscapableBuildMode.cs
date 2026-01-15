using System;
using Data.Buildable;
using Deck.Services.Building.Events;
using Deck.Services.Escapable;
using Deck.Services.UI;
using EventManager;
using Services.Building;
using Systems.SystemInput.Events;
using Utility;

namespace UI.Building.BuildMode
{
    public abstract class DeckEscapableBuildMode : IDeckEscapable
    {
        public bool HasEscaped { get; private set; }

        protected DeckServiceBuilding BuildingService;
        protected DeckBuildable Buildable;
        protected DeckBuildingPage BuildingPage;
        private Action _onEscape;
        private bool _canMoveBuild;

        public void Initialize(Action onEscape, DeckBuildable buildable, DeckBuildingPage buildingPage)
        {
            if (Buildable == buildable)
            {
                return;
            }

            Buildable = buildable;
            BuildingPage = buildingPage;

            _onEscape = onEscape;

            DeckEventManager.Register<DeckEventOnLeftClickDown>(InternalOnLeftClickDown);
            DeckEventManager.Register<DeckEventOnLeftClickUp>(InternalOnLeftClickUp);
            DeckEventManager.Register<DeckEventOnMouseMove>(InternalOnMouseMove);

            BuildingService = Services.DeckServiceProvider.GetService<DeckServiceBuilding>();
            DeckEventOnBuildModeStarted.Create().Send();

            Services.DeckServiceProvider.GetService<DeckServiceEscapable>().RegisterEscapable(this);

            InternalOnInitialize();
        }

        public void OnEscapeRequested()
        {
            if (HasEscaped)
            {
                return;
            }

            DeckEventManager.Unregister<DeckEventOnLeftClickDown>(InternalOnLeftClickDown);
            DeckEventManager.Unregister<DeckEventOnLeftClickUp>(InternalOnLeftClickUp);
            DeckEventManager.Unregister<DeckEventOnMouseMove>(InternalOnMouseMove);

            HasEscaped = true;
            _onEscape?.Invoke();

            DeckEventOnBuildModeStopped.Create().Send();
        }

        protected abstract void InternalOnInitialize();

        protected abstract void InternalOnLeftClickDown(DeckEventOnLeftClickDown obj);

        protected abstract void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj);

        protected abstract void InternalOnMouseMove(DeckEventOnMouseMove obj);
    }
}