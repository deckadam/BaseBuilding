using Deck.EventManager;

namespace Deck.Utility.Constants.Hotkey.Events
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