using Deck.EventManager;

namespace Deck.Components.Building
{
    public class DeckEventNotificationRequested : DeckEvent
    {
        public string message { get; private set; }

        public static DeckEventNotificationRequested Create(string message)
        {
            return new DeckEventNotificationRequested
            {
                message = message
            };
        }
    }
}