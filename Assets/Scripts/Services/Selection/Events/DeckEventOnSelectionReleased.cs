using Base;
using EventManager;

namespace Services.Selection.Events
{
    public class DeckEventOnSelectionReleased : IDeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnSelectionReleased Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}