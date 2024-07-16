using Cysharp.Threading.Tasks;
using Deck.EventManager;

namespace Deck.UI.Building
{
    public class DeckBuildingUI : DeckUIBase
    {
        public override void Initialize()
        {
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
    }
}