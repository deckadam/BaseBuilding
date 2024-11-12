namespace Deck.Services
{
    public interface IDeckEscapable
    {
        bool HasEscaped { get; }
        void OnEscapeRequested();
    }
}