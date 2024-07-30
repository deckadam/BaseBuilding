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

        public DeckEscapableBuildMode(Action onEscape, Action<Vector3> onBuild)
        {
            _onEscape = onEscape;
            _onBuild = onBuild;
            DeckEventManager.Register<DeckEventOnLeftClick>(OnLeftClick);
        }

        public void OnCloseRequested()
        {
            DeckEventManager.Unregister<DeckEventOnLeftClick>(OnLeftClick);
            _isEscaped = true;
            _onEscape?.Invoke();
        }

        private void OnLeftClick(DeckEventOnLeftClick obj)
        {
            _onBuild?.Invoke(obj.position);
        }

        public bool IsEscaped()
        {
            return _isEscaped;
        }
    }
}