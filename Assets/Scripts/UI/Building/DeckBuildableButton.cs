using Base;
using Data.Buildable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;

        private DeckBuildable _buildable;
        private DeckBuildingPage _buildingPage;

        public void Initialize(DeckBuildingPage buildingPage, DeckBuildable buildable)
        {
            _buildingPage = buildingPage;
            _buildable = buildable;
            icon.sprite = _buildable.Icon;
            nameText.text = buildable.VisibleName;
            //TODO: Multiple price support
            priceText.text = buildable.Prices[0].Amount.ToString();
        }

        public void OnClick()
        {
            _buildingPage.OnBuildableSelected(_buildable);
        }
    }
}