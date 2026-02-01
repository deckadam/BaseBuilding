using System;
using Data.Buildable;
using EventManager;
using Services;
using Services.Building;
using Services.Building.Events;
using Services.Escapable;
using Services.UI;
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
        protected bool _isDown;

        private Action _onEscape;
        private bool _canMoveBuild;
        private bool _checking;

        public void Initialize(Action onEscape, DeckBuildable buildable, DeckBuildingPage buildingPage)
        {
            if (Buildable == buildable)
            {
                return;
            }

            Buildable = buildable;
            BuildingPage = buildingPage;

            _onEscape = onEscape;

            DeckEventManager.Register<DeckEventOnLeftClickDown>(OnLeftClickDown);
            DeckEventManager.Register<DeckEventOnLeftClickUp>(OnLeftClickUp);
            DeckEventManager.Register<DeckEventOnMouseMove>(OnMouseMove);
            DeckEventManager.Register<DeckEventOnRightClick>(OnRightClick);
            DeckEventManager.Register<DeckEventOnMiddleScroll>(OnMiddleScroll);

            BuildingService = DeckServiceProvider.GetService<DeckServiceBuilding>();
            DeckEventOnBuildModeStarted.Create().Send();

            DeckServiceProvider.GetService<DeckServiceEscapable>().RegisterEscapable(this);

            InternalOnInitialize();
            _checking = true;
        }

        public void OnEscapeRequested()
        {
            if (HasEscaped)
            {
                return;
            }

            DeckEventManager.Unregister<DeckEventOnLeftClickDown>(OnLeftClickDown);
            DeckEventManager.Unregister<DeckEventOnLeftClickUp>(OnLeftClickUp);
            DeckEventManager.Unregister<DeckEventOnMouseMove>(OnMouseMove);
            DeckEventManager.Unregister<DeckEventOnRightClick>(OnRightClick);
            DeckEventManager.Unregister<DeckEventOnMiddleScroll>(OnMiddleScroll);

            HasEscaped = true;
            _onEscape?.Invoke();

            DeckEventOnBuildModeStopped.Create().Send();
        }

        private void OnRightClick(DeckEventOnRightClick obj)
        {
            _checking = false;
            if (!ShouldEscapeFullyOnRightClick)
            {
                PartialCancel();
            }
            else
            {
                BuildingService.ClearAll();
                OnEscapeRequested();
            }
        }

        private void OnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
            _checking = true;
            InternalOnLeftClickDown();
        }

        private void OnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            if (!_checking)
            {
                return;
            }

            InternalOnLeftClickUp();
        }

        private void OnMouseMove(DeckEventOnMouseMove obj)
        {
            if (!_checking)
            {
                return;
            }

            InternalOnMouseMove();
        }

        private void OnMiddleScroll(DeckEventOnMiddleScroll obj)
        {
            if (!_checking)
            {
                return;
            }

            InternalOnMiddleScroll();
        }

        protected virtual void PartialCancel()
        {
        }

        protected abstract void InternalOnInitialize();
        protected abstract void InternalOnLeftClickDown();
        protected abstract void InternalOnLeftClickUp();
        protected abstract void InternalOnMouseMove();
        protected abstract void InternalOnMiddleScroll();

        public bool CanBeEscapedWithRightClick => true;
        public virtual bool ShouldEscapeFullyOnRightClick => true;
    }
}