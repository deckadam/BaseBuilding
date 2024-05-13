using Deck.UI.InGame.AI;

namespace Deck.Agent.Enemy
{
    public class DeckAgentEnemy : DeckAgent
    {
        protected override void AfterInitialize()
        {
            if (TryGetComponent(out DeckAI ai))
            {
                ai.StartAi();
            }
        }
    }
}