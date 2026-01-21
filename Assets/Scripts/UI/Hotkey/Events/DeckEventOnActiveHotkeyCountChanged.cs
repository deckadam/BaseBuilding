using EventManager;

namespace UI.Hotkey.Events
{
    public struct DeckEventOnActiveHotkeyCountChanged : IDeckEvent
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