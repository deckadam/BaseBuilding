namespace Deck.Base
{
    public class DeckAgentHumanoid : DeckAgent
    {
        protected override void InternalRequestDestroy()
        {
            instanceProvider.ReturnAgent(this);
        }

        public void Sit()
        {
        }

        public void GetUp()
        {
        }
    }
}