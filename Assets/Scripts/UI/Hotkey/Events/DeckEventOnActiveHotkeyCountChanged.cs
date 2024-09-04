using Deck.EventManager;

namespace Deck.InGame.Agent.Building.Hotkey.Events
{
    public class DeckEventOnActiveHotkeyCountChanged : DeckEvent
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