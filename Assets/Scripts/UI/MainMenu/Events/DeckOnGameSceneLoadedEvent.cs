using Deck.EventManager;

namespace Deck.UI
{
    public class DeckOnGameSceneLoadedEvent : DeckEvent
    {
        public static DeckOnGameSceneLoadedEvent Create()
        {
            return new DeckOnGameSceneLoadedEvent();
        }
    }
}