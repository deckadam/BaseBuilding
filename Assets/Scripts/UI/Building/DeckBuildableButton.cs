using Base;
using Data.Buildable;
using EventManager;
using Services;
using Services.Building.Events;
using Services.Finder;
using TMPro;
using UI.MainMenu.Events;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI limitText;

        private DeckBuildable _buildable;
        private DeckBuildingPage _buildingPage;
        private int _maxLimit;

        public void Initialize(DeckBuildingPage buildingPage, DeckBuildable buildable)
        {
            _buildingPage = buildingPage;
            _buildable = buildable;
            icon.sprite = _buildable.Icon;
            nameText.text = buildable.VisibleName;
            //TODO: Multiple price support
            priceText.text = buildable.Prices[0].Amount.ToString();

            DeckEventManager.Register<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Register<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Register<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);
        }

        private void OnDestroy()
        {
            DeckEventManager.Unregister<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Unregister<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Unregister<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);
        }

        private void OnGameSceneLoaded(DeckEventOnGameSceneLoaded obj)
        {
            if (_buildable.IsLimited)
            {
                var builtCount = 0;
                if (DeckServiceProvider.GetService<DeckServiceFinder>().TryGetAgentsWithPrefabId(_buildable.Agent.PrefabId, out var agents))
                {
                    builtCount = agents.Count;
                }

                limitText.gameObject.SetActive(true);
                limitText.text = (_buildable.Limit - builtCount).ToString();
                _maxLimit = _buildable.Limit - builtCount;
                AdjustButtonStatus();
            }
            else
            {
                limitText.gameObject.SetActive(false);
            }
        }

        private void OnAnythingBuilt(DeckEventOnAnythingBuilt obj)
        {
            if (!obj.building.PrefabId.Equals(_buildable.Agent.PrefabId)) return;

            _maxLimit--;
            AdjustButtonStatus();
        }

        private void OnAnythingDestroyed(DeckEventOnAnythingDestroyed obj)
        {
            if (!obj.building.PrefabId.Equals(_buildable.Agent.PrefabId)) return;

            _maxLimit++;
            AdjustButtonStatus();
        }

        public void OnClick()
        {
            _buildingPage.OnBuildableSelected(_buildable);
        }

        private void AdjustButtonStatus()
        {
            if (_maxLimit == 0)
            {
                button.interactable = false;
            }
            else
            {
                button.interactable = true;
            }

            limitText.text = _maxLimit.ToString();
            _buildable.SetCurrentLimit(_maxLimit);
        }
    }
}