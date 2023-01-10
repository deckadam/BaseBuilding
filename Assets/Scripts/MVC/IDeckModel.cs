using System;

namespace Deck.MVC
{
    public interface IDeckModel<T, J>
    {
        void Initialize();
        void AddData(T data);
        void RemoveData(T data);
        J Getter();
        void Setter(J obj);
        void Register(Action<IDeckModel<T, J>> listener);
        void Unregister(Action<IDeckModel<T, J>> listener);
        void ClearListeners();
    }
}