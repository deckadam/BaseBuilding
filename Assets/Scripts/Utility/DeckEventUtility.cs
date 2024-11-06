using Deck.EventManager;

namespace Deck.Utility.Logger
{
    public static class DeckEventUtility
    {
        public static void Send<T>(this T obj) where T : IDeckEvent
        {
            DeckEventManager.Send(obj);
        }
    }
}