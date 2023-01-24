using Deck.EventManager;

namespace Deck.UI.Hotkey.Events
{
    public class DeckOnHotkeySelected : DeckEvent
    {
        public int index { get; private set; }

        public static DeckOnHotkeySelected Create(int index)
        {
            return new()
            {
                index = index
            };
        }
    }
}