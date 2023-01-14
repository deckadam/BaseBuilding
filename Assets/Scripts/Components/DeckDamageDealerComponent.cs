using Deck.Data.Damage;
using Deck.Player;

namespace Deck.Components
{
    public class DeckDamageDealerComponent : IDeckComponent
    {
        private DeckAgent _bindedAgent;
        private DeckDamageData _damageData;

        public void DealDamage()
        {
        }

        public void Initialize(DeckAgent agent)
        {
            _bindedAgent = agent;
            _damageData = _bindedAgent.GetAgentData().damageData;
        }

        public void DeInitialize()
        {
        }

        public DeckAgent GetAgent()
        {
            return null;
        }
    }
}