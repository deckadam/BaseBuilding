using System;
using Deck.Data.Buildable;
using Deck.Utility.Poolable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.InGame.Agent.Building.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;

        private DeckBuildable _buildable;
        private Action<DeckBuildable> _onClick;

        public void Initialize(Action<DeckBuildable> onClick, DeckBuildable buildable)
        {
            _onClick = onClick;
            _buildable = buildable;
            icon.sprite = _buildable.Icon;
            nameText.text = buildable.VisibleName;
        }

        public void OnClick()
        {
            _onClick?.Invoke(_buildable);
        }
    }
}