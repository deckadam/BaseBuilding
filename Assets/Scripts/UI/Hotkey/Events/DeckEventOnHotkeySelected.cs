using Deck.EventManager;

namespace Deck.Components.Building.Hotkey.Events
{
    public class DeckEventOnHotkeySelected : IDeckEvent
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