using System;
using System.Collections.Generic;

namespace Deck.MVC.DeckMVCController
{
    public interface IDeckModel<T>
    {
        IEnumerable<T> Getter();
        void Setter(IEnumerable<T> obj);
        void Register(Action<IDeckModel<T>> listener);
        void Unregister(Action<IDeckModel<T>> listener);
        void ClearListeners();
    }
}