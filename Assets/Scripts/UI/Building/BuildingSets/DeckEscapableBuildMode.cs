using System;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Services;
using UnityEngine;

namespace Deck.UI.Building.BuildingSets
{
    public class DeckEscapableBuildMode : IDeckEscapable
    {
        private Action _onEscape;
        private Action<Vector3> _onBuild;
        private bool _isEscaped;
        private bool _canMoveBuild;

        public DeckEscapableBuildMode(Action onEscape, Action<Vector3> onBuild, bool canMoveBuild = false)
        {
            _onEscape = onEscape;
            _onBuild = onBuild;
            DeckEventManager.Register<DeckEventOnLeftClick>(OnLeftClick);

            _canMoveBuild = canMoveBuild;
            if (_canMoveBuild)
            {
                DeckEventManager.Register<DeckEventOnMouseMove>(OnMouseMove);
            }
        }

        public void OnCloseRequested()
        {
            DeckEventManager.Unregister<DeckEventOnLeftClick>(OnLeftClick);

            if (_canMoveBuild)
            {
                DeckEventManager.Unregister<DeckEventOnMouseMove>(OnMouseMove);
            }

            _isEscaped = true;
            _onEscape?.Invoke();
        }

        private void OnLeftClick(DeckEventOnLeftClick obj)
        {
            _onBuild?.Invoke(obj.position);
        }

        private void OnMouseMove(DeckEventOnMouseMove obj)
        {
            _onBuild?.Invoke(obj.position);
        }

        public bool IsEscaped()
        {
            return _isEscaped;
        }
    }
}