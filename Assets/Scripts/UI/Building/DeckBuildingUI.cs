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
        private BuildingSet _currentlyShownBuildingSet;

        public override void Initialize()
        {
            foreach (var deckBuildingSet in buildingSets)
            {
                deckBuildingSet.button.Initialize(this, deckBuildingSet.page);
                deckBuildingSet.page.Initialize(this, deckBuildingSet.button);
            }

            DeckEventManager.Register<DeckOnGameSceneLoadedEvent>(OnEventAppear);
            DeckEventManager.Register<DeckOnMainMenuDisappearEvent>(OnEventAppear);
            DeckEventManager.Register<DeckOnMainMenuAppearedEvent>(OnEventDisappear);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnGameSceneLoadedEvent>(OnEventAppear);
            DeckEventManager.Unregister<DeckOnMainMenuDisappearEvent>(OnEventAppear);
            DeckEventManager.Unregister<DeckOnMainMenuAppearedEvent>(OnEventDisappear);
        }

        private void OnEventAppear(DeckEvent _)
        {
            Appear().Forget();
        }


        private void OnEventDisappear(DeckEvent _)
        {
            Disappear().Forget();
        }

        public void OnBuildingSetButtonClicked(DeckBuildingButton button)
        {
            foreach (var deckBuildingSet in buildingSets)
            {
                if (deckBuildingSet.button == button)
                {
                    _currentlyShownBuildingSet = deckBuildingSet;
                    deckBuildingSet.page.Appear().Forget();
                }
                else
                {
                    deckBuildingSet.page.Disappear().Forget();
                }
            }

            Deck.GetService<DeckServiceEscapable>().RegisterEscapable(this);
        }

        public override void OnCloseRequested()
        {
            _currentlyShownBuildingSet.page.Disappear().Forget();
            _currentlyShownBuildingSet.button.Disappear().Forget();
        }

        [Serializable]
        private struct BuildingSet
        {
            public DeckBuildingButton button;
            public DeckBuildingPage page;
        }
    }
}