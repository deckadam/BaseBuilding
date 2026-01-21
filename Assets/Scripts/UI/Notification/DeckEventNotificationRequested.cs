using EventManager;

namespace UI.Notification
{
    public struct DeckEventNotificationRequested : IDeckEvent
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