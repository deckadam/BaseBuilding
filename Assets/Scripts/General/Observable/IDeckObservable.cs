using System;

namespace Deck.Generalnterfaces
{
	public interface IDeckObservable<out T>
	{
		void AddListener(Action<T> listener);
		void RemoveListener(Action<T> listener);
		void ClearListeners();
	}
}