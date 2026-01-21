using EventManager;

namespace UI.Hotkey.Events
{
    public struct DeckEventOnHotkeySelected : IDeckEvent
    {
        public int index { get; private set; }

        public static DeckEventOnHotkeySelected Create(int index)
        {
            return new DeckEventOnHotkeySelected
            {
                index = index
            };
        }
    }
}