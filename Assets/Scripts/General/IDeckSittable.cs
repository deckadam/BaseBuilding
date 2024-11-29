using Deck.Base;

namespace Deck.General
{
    public interface IDeckSittable
    {
        void OnSit(DeckAgentHumanoid humanoid);
        void OnGetUp(DeckAgentHumanoid humanoid);
    }
}