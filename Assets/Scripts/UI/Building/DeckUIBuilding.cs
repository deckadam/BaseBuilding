using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.Buildable;
using Data.Buildable.Data;
using EventManager;
using Services;
using Services.Escapable;
using Services.UI;
using UI.Building.Data;
using UI.MainMenu.Events;
using UnityEngine;
using Zenject;

namespace UI.Building
{
    public class DeckUIBuilding : DeckUIBase, IDeckEscapable
    {
        [SerializeField] private RectTransform buttonsContainer;
        [SerializeField] private RectTransform pagesContainer;

        private DeckServiceEscapable _serviceEscapable;
        private DeckBuildingButton _currentButton;

        private List<DeckBuildingSet> _buildingSets;
        private DeckBuildable[] _buildables;

        private Dictionary<DeckBuildableCategory, List<DeckBuildable>> _categories;

        [Inject]
        private void Inject(List<DeckBuildingSet> buildingSets, DeckBuildable[] buildables)
        {
            _buildingSets = buildingSets;
            _buildables = buildables;
        }

        public override void Initialize()
        {
            _categories = new Dictionary<DeckBuildableCategory, List<DeckBuildable>>();
            foreach (var deckBuildable in _buildables)
            {
                if (!_categories.TryGetValue(deckBuildable.Category, out var category))
                {
                    category = new List<DeckBuildable> { deckBuildable };
                    _categories[deckBuildable.Category] = category;
                }
                else
                {
                    category.Add(deckBuildable);
                }

                Debug.LogError(deckBuildable.Category.name);
            }

            foreach (var buildingSet in _buildingSets)
            {
                var page = InstanceProvider.RentUIElement(buildingSet.Page.GetType()).GetComponent<DeckBuildingPage>();
                page.rectTransform.SetParent(pagesContainer, false);

                var button = InstanceProvider.RentUIElement(buildingSet.Button.GetType()).GetComponent<DeckBuildingButton>();
                button.rectTransform.SetParent(buttonsContainer, false);

                page.Initialize(_categories[buildingSet.Category]);
                button.Initialize(this, page);
            }

            _serviceEscapable = DeckServiceProvider.GetService<DeckServiceEscapable>();

            DeckEventManager.Register<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Register<DeckEventOnMainMenuDisappear>(OnMainMenuDisappear);
            DeckEventManager.Register<DeckEventOnMainMenuAppeared>(OnMainMenuAppeared);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnGameSceneLoaded>(OnGameSceneLoaded);
            DeckEventManager.Unregister<DeckEventOnMainMenuDisappear>(OnMainMenuDisappear);
            DeckEventManager.Unregister<DeckEventOnMainMenuAppeared>(OnMainMenuAppeared);
        }

        public void PageOpenRequested(DeckBuildingButton button)
        {
            _serviceEscapable.ClearEscapables();
            _currentButton = button;
        }

        private void OnGameSceneLoaded(DeckEventOnGameSceneLoaded obj)
        {
            OnEventAppear();
        }

        private void OnMainMenuDisappear(DeckEventOnMainMenuDisappear obj)
        {
            OnEventAppear();
        }

        private void OnMainMenuAppeared(DeckEventOnMainMenuAppeared obj)
        {
            OnEventDisappear();
        }

        private void OnEventAppear()
        {
            Appear().Forget();
            HasEscaped = false;
        }

        private void OnEventDisappear()
        {
            Disappear().Forget();
            HasEscaped = true;
        }

        public bool CanBeEscapedWithRightClick => false;
        public bool HasEscaped { get; private set; }
    }
}