using Deck.Player;

namespace Deck.Components
{
    public interface IDeckComponent
    {
        void Initialize(DeckAgent agent);
        void DeInitialize();
        DeckAgent GetAgent();
    }
}