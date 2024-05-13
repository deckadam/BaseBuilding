namespace Deck.Commands
{
    public interface IDeckAnimationSetBool : IDeckComponent
    {
        void Animate(string name, bool value);
    }
}