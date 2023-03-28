using Deck.EventManager;

namespace Deck.UI
{
    public class DeckNotificationRequestedEvent : DeckEvent
    {
        public string message { get; private set; }

        public static DeckNotificationRequestedEvent Create(string message)
        {
            return new DeckNotificationRequestedEvent
            {
                message = message
            };
        }
    }
}