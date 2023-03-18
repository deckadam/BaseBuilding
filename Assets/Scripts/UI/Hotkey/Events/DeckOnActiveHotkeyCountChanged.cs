using Deck.EventManager;

namespace Deck.Utility.Constants.Hotkey.Events
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