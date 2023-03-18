using Data.Component;
using Deck.Agent;

namespace Deck.Map
{
    public class DeckAgentBlockade : DeckAgent
    {
        //Dummy
        private void Awake()
        {
            SetComponentDatas(new DeckComponentData[0]);
            SetComponents();
        }

        public override void RequestDeath()
        {
            DeInitialize();
        }
    }
}