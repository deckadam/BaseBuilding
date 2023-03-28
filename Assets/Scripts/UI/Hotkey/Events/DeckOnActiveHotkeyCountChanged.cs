using Deck.EventManager;

namespace Deck.UI.Hotkey.Events
{
    public class DeckOnActiveHotkeyCountChanged : DeckEvent
    {
        public int count { get; private set; }

        public static DeckOnActiveHotkeyCountChanged Create(int count)
        {
            return new DeckOnActiveHotkeyCountChanged()
            {
                count = count
            };
        }
    }
}