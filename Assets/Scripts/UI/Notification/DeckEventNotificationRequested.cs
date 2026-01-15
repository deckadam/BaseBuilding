using EventManager;

namespace Deck.UI.Notification
{
    public class DeckEventNotificationRequested : IDeckEvent
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