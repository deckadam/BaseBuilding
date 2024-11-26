using Deck.Base;

namespace Deck.Components
{
    public interface IDeckAnimationSetBool : IDeckComponent
    {
        void Animate(string name, bool value);
    }
}