using Deck.EventManager;

namespace Deck.InGame.Agent.Building
{
    public class DeckEventOnGameSceneLoaded : DeckEvent
    {
        public static DeckEventOnGameSceneLoaded Create()
        {
            return new DeckEventOnGameSceneLoaded();
        }
    }
}