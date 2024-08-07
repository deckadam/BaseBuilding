using System;
using Deck.Data.Buildable;
using Deck.Utility.Poolable;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;

        private DeckBuildable _buildable;
        private Action<DeckBuildable> _onClick;

        public void Initialize(Action<DeckBuildable> onClick, DeckBuildable buildable)
        {
            _onClick = onClick;
            _buildable = buildable;
            icon.sprite = _buildable.Icon;
        }

        public void OnClick()
        {
            _onClick?.Invoke(_buildable);
        }
    }
}