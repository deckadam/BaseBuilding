using Base;
using EventManager;

namespace Services.Selection.Events
{
    public struct DeckEventOnAgentSelected : IDeckEvent
    {
        public DeckAgent agent { get; private set; }

        public static DeckEventOnAgentSelected Create(DeckAgent agent)
        {
            return new()
            {
                agent = agent
            };
        }
    }
}