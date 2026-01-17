using Base;
using EventManager;

namespace Services.Selection.Events
{
    public class DeckEventOnAgentPossessed : IDeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnAgentPossessed Crate(DeckAgent agent)
        {
            return new DeckEventOnAgentPossessed
            {
                agent = agent
            };
        }
    }
}