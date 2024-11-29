using System;

namespace Deck.General
{
	public interface IDeckObservable<out T>
	{
		void AddListener(Action<T> listener);
		void RemoveListener(Action<T> listener);
		void ClearListeners();
	}
}