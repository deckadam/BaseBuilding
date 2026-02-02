using Base;
using Data.Currency;
using EventManager;
using Instancing;
using Services;
using Services.Building.Buildable;
using Services.Building.Buildable.Data.Parameter.Implementations;
using Services.Building.Events;
using Services.Finder;
using TMPro;
using UI.Generic.PriceText;
using UI.MainMenu.Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Building
{
    public class DeckBuildableButton : DeckUIElement
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform priceTextContainer;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private TextMeshProUGUI limitText;

        private DeckBuildable _buildable;
        private DeckBuildingPage _buildingPage;
        private DeckInstanceProvider _instanceProvider;
        private int _maxLimit;
        private bool _isLimited;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public void Initialize(DeckBuildingPage buildingPage, DeckBuildable buildable)
        {
            _buildingPage = buildingPage;
            _buildable = buildable;

            if (_buildable.TryGetParameter(out DeckBuildableParameterIcon iconParameter))
            {
                icon.sprite = iconParameter.GetValue<Sprite>();
            }

            nameText.text = buildable.VisibleName;

            if (_buildable.TryGetParameter(out DeckBuildableParameterPrice priceParameter))
            {
                foreach (var deckPrice in priceParameter.GetValue<DeckPrice[]>())
                {
                    var newPriceDisplay = _instanceProvider.RentUIElement<DeckUIPriceDisplay>();
                    newPriceDisplay.Initialize(deckPrice, priceTextContainer);
                }
            }

            if (_buildable.TryGetParameter(out DeckBuildableParameterLimited _))
            {
                _isLimited = true;
            }
            else
            {
                limitText.gameObject.SetActive(false);
                return;
            }

            DeckEventManager.Register<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Register<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Register<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);
        }

        private void OnDestroy()
        {
            if (!_isLimited)
            {
                return;
            }

            DeckEventManager.Unregister<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Unregister<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Unregister<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);
        }

        private void OnGameSceneLoaded(DeckEventOnGameSceneLoaded obj)
        {
            if (_buildable.TryGetParameter(out DeckBuildableParameterLimited parameterLimited))
            {
                var builtCount = 0;
                if (DeckServiceProvider.GetService<DeckServiceFinder>().TryGetAgentsWithPrefabId(_buildable.Agent.PrefabId, out var agents))
                {
                    builtCount = agents.Count;
                }

                var limit = parameterLimited.GetMaxLimit();
                limitText.gameObject.SetActive(true);
                limitText.text = (limit - builtCount).ToString();
                _maxLimit = limit - builtCount;
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

            if (_buildable.TryGetParameter(out DeckBuildableParameterLimited parameter))
            {
                parameter.SetCurrentLimit(_maxLimit);
            }
        }
    }
}