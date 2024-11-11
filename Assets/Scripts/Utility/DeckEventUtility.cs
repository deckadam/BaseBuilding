using Deck.EventManager;

namespace Deck.Utility
{
    public static class DeckEventUtility
    {
        public static void Send<T>(this T obj) where T : IDeckEvent
        {
            DeckEventManager.Send(obj);
        }
    }
}