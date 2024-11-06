using Deck.EventManager;

namespace Deck.Components.Building.Hotkey.Events
{
    public class DeckEventOnActiveHotkeyCountChanged : IDeckEvent
    {
        public int count { get; private set; }

        public static DeckEventOnActiveHotkeyCountChanged Create(int count)
        {
            return new DeckEventOnActiveHotkeyCountChanged()
            {
                count = count
            };
        }
    }
}