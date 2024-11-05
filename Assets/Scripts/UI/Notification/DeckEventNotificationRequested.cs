using Deck.EventManager;
using UnityEngine;

namespace Deck.Components.Building
{
    public class DeckEventNotificationRequested : DeckEvent
    {
        public string message { get; private set; }

        public static DeckEventNotificationRequested Create(string message)
        {
            Debug.LogError("New notif");
            return new DeckEventNotificationRequested
            {
                message = message
            };
        }
    }
}