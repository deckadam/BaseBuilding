using Deck.EventManager;

namespace GameEvents
{
    public class DeckOnMapLoadedEvent : DeckEvent
    {
        public static DeckOnMapLoadedEvent Create()
        {
            return new DeckOnMapLoadedEvent();
        }
    }
}