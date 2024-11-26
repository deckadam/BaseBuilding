namespace Deck.Services.UI
{
    public interface IDeckEscapable
    {
        bool HasEscaped { get; }
        void OnEscapeRequested();
    }
}