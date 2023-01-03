namespace Deck.Player
{
    public interface IDeckCorePlayerSystem
    {
        void Initialize(DeckCoreAgent deckCoreAgent);
        void DeInitialize();
    }
}