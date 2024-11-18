using System;
using Deck.Data.Buildable;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Services;
using Deck.Services.Building;
using Deck.Services.Implementations.Escapable;
using Deck.Utility;
using UnityEngine;

namespace Deck.UI.Building.BuildMode
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
            Buildable = buildable;
            BuildingPage = buildingPage;
            
            _onEscape = onEscape;

            DeckEventManager.Register<DeckEventOnLeftClickDown>(InternalOnLeftClickDown);
            DeckEventManager.Register<DeckEventOnLeftClickUp>(InternalOnLeftClickUp);
            DeckEventManager.Register<DeckEventOnMouseMove>(InternalOnMouseMove);

            BuildingService = Deck.GetService<DeckServiceBuilding>();
            DeckEventOnBuildModeStarted.Create().Send();

            Deck.GetService<DeckServiceEscapable>().RegisterEscapable(this);

            InternalOnInitialize();

            Debug.LogError(GetType() + "  initialized");
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
            Debug.LogError(GetType() + "  deinitialized");
        }

        protected abstract void InternalOnInitialize();

        protected abstract void InternalOnLeftClickDown(DeckEventOnLeftClickDown obj);

        protected abstract void InternalOnLeftClickUp(DeckEventOnLeftClickUp obj);

        protected abstract void InternalOnMouseMove(DeckEventOnMouseMove obj);
    }
}