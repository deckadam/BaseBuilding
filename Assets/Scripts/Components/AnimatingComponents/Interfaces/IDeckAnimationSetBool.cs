using Base;

namespace Components.AnimatingComponents.Interfaces
{
    public interface IDeckAnimationSetBool : IDeckComponent
    {
        void Animate(string name, bool value);
    }
}