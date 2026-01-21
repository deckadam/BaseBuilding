using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventManager;
using Services.Escapable;
using Services.UI;
using UI.MainMenu.Events;
using UnityEngine;

namespace UI.Building
{
    public class DeckUIBuilding : DeckUIBase, IDeckEscapable
    {
        [SerializeField] private List<BuildingSet> buildingSets;
        [SerializeField] private RectTransform buttonsContainer;
        [SerializeField] private RectTransform pagesContainer;

        private DeckServiceEscapable _serviceEscapable;
        private DeckBuildingButton _currentButton;

        public override void Initialize()
        {
            foreach (var buildingSet in buildingSets)
            {
                var page = InstanceProvider.RentUIElement(buildingSet.page.GetType()).GetComponent<DeckBuildingPage>();
                page.rectTransform.SetParent(pagesContainer, false);

                var button = InstanceProvider.RentUIElement(buildingSet.button.GetType()).GetComponent<DeckBuildingButton>();
                button.rectTransform.SetParent(buttonsContainer, false);

                page.Initialize();
                button.Initialize(this, page);
            }

            _serviceEscapable = Services.DeckServiceProvider.GetService<DeckServiceEscapable>();

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

        [Serializable]
        private struct BuildingSet
        {
            public DeckBuildingButton button;
            public DeckBuildingPage page;
        }

        public bool HasEscaped { get; private set; }
    }
}