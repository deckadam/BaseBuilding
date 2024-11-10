using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.EventManager;
using Deck.Services;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.Components.Building.Building
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

                page.Initialize(this, buildingSet.button);
                button.Initialize(this, page);
            }

            _serviceEscapable = Deck.GetService<DeckServiceEscapable>();

            DeckEventManager.Register<DeckEventOnGameSceneLoaded>(OnEventAppear);
            DeckEventManager.Register<DeckEventOnMainMenuDisappear>(OnEventAppear);
            DeckEventManager.Register<DeckEventOnMainMenuAppeared>(OnEventDisappear);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnGameSceneLoaded>(OnEventAppear);
            DeckEventManager.Unregister<DeckEventOnMainMenuDisappear>(OnEventAppear);
            DeckEventManager.Unregister<DeckEventOnMainMenuAppeared>(OnEventDisappear);
        }

        public void PageOpenRequested(DeckBuildingButton button)
        {
            _serviceEscapable.ClearEscapables();
            _currentButton = button;
        }

        private void OnEventAppear(IDeckEvent _)
        {
            Appear().Forget();
            HasEscaped = false;
        }

        private void OnEventDisappear(IDeckEvent _)
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