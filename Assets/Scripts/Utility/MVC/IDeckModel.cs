using System;

namespace Deck.MVC
{
    public interface IDeckModel<T, J>
    {
        void AddData(T param1);
        void RemoveData(T param1);
        J Getter();
        void Setter(J param1);
        void Register(Action<IDeckModel<T, J>> listener);
        void Unregister(Action<IDeckModel<T, J>> listener);
        void ClearListeners();
    }
}