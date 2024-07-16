using Deck.EventManager;

namespace Deck.UI
{
    public class DeckOnMainMenuAppearedEvent : DeckEvent
    {
        public static DeckOnMainMenuAppearedEvent Create()
        {
            return new DeckOnMainMenuAppearedEvent();
        }
    }
}