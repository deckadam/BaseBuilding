using System;
using Deck.Data.Buildable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.Components.Building.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;

        private DeckBuildable _buildable;
        private Action<DeckBuildable> _onClick;

        public void Initialize(Action<DeckBuildable> onClick, DeckBuildable buildable)
        {
            _onClick = onClick;
            _buildable = buildable;
            icon.sprite = _buildable.Icon;
            nameText.text = buildable.VisibleName;
            //TODO: Multiple price support
            priceText.text = buildable.Prices[0].Amount.ToString();
        }

        public void OnClick()
        {
            _onClick?.Invoke(_buildable);
        }
    }
}