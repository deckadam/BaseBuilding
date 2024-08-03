using Deck.EventManager;

namespace Deck.UI.Hotkey.Events
{
    public class DeckEventOnHotkeySelected : DeckEvent
    {
        public int index { get; private set; }

        public static DeckEventOnHotkeySelected Create(int index)
        {
            return new()
            {
                index = index
            };
        }
    }
}