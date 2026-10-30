using EventManager;

namespace InGame.Agent.Raid.Event
{
    public struct DeckEventOnRaiderDeath : IDeckEvent
    {
        public DeckAgentRaider raider { get; private set; }

        public static DeckEventOnRaiderDeath Create(DeckAgentRaider raider)
        {
            return new DeckEventOnRaiderDeath()
            {
                raider = raider
            };
        }
    }
}