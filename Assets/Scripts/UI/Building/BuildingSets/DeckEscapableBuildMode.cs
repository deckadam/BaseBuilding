using System;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Services;
using UnityEngine;

namespace Deck.Components.Building.Building.BuildingSets
{
    public class DeckEscapableBuildMode : IDeckEscapable
    {
        private Action<Vector3, Quaternion> _onBuildWithRotation;
        private readonly Action<Vector3[]> _onBuild;
        private readonly Action _onEscape;
        
        public bool HasEscaped { get; private set; }
        
        private readonly bool _canMoveBuild;
        private bool _isClosed;

        protected DeckEscapableBuildMode()
        {
            throw new NotImplementedException();
        }

        public DeckEscapableBuildMode(Action onEscape, Action<Vector3[]> onBuild, bool canMoveBuild = false)
        {
            _onEscape = onEscape;
            _onBuild = onBuild;
            DeckEventManager.Register<DeckEventOnLeftClickDown>(OnLeftClickDown);
            DeckEventManager.Register<DeckEventOnLeftClickUp>(OnLeftClickUp);

            _canMoveBuild = canMoveBuild;
            if (_canMoveBuild)
            {
                DeckEventManager.Register<DeckEventOnMouseMove>(OnMouseMove);
            }
        }

        public void OnCloseRequested()
        {
            if (_isClosed)
            {
                return;
            }

            DeckEventManager.Unregister<DeckEventOnLeftClickDown>(OnLeftClickDown);
            DeckEventManager.Unregister<DeckEventOnLeftClickUp>(OnLeftClickUp);

            if (_canMoveBuild)
            {
                DeckEventManager.Unregister<DeckEventOnMouseMove>(OnMouseMove);
            }

            HasEscaped = true;
            _onEscape?.Invoke();

            _isClosed = true;
        }

        protected virtual void OnLeftClickDown(DeckEventOnLeftClickDown obj)
        {
        }

        protected virtual void OnLeftClickUp(DeckEventOnLeftClickUp obj)
        {
            _onBuild?.Invoke(new[]
            {
                obj.position
            });
        }

        protected virtual void OnMouseMove(DeckEventOnMouseMove obj)
        {
            _onBuild?.Invoke(new[]
            {
                obj.position
            });
        }
    }
}