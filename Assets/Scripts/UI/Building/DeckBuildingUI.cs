using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Deck.EventManager;
using Deck.Services;
using Services.Implementations.Escapable;
using UnityEngine;

namespace Deck.UI.Building
{
    public class DeckBuildingUI : DeckUIBase, IDeckEscapable
    {
        [SerializeField] private List<BuildingSet> buildingSets;
        [SerializeField] private RectTransform buttonsContainer;
        [SerializeField] private RectTransform pagesContainer;

        public override void Initialize()
        {
            foreach (var buildingSet in buildingSets)
            {
                var page = uiPool.Rent<DeckBuildingPage>(buildingSet.page.PrefabId);
                page.rectTransform.SetParent(pagesContainer, false);
                
                var button = uiPool.Rent<DeckBuildingButton>(buildingSet.button.PrefabId);
                button.rectTransform.SetParent(buttonsContainer, false);
                
                page.Initialize(this, buildingSet.button);
                button.Initialize(this, page);
            }

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

        private void OnEventAppear(DeckEvent _)
        {
            Appear().Forget();
        }

        private void OnEventDisappear(DeckEvent _)
        {
            Disappear().Forget();
        }

        [Serializable]
        private struct BuildingSet
        {
            public DeckBuildingButton button;
            public DeckBuildingPage page;
        }
    }
}