using Deck.Player;

namespace Deck.Components
{
    public interface IDeckComponent
    {
        void Initialize(DeckAgent deckCoreAgent);
        void DeInitialize();
        DeckAgent GetAgent();
    }
}