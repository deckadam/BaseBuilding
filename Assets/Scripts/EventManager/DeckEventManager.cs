using System;

namespace EventManager
{
    public static class DeckEventManager
    {
        public static void Register<T>(Action<T> obj) where T : IDeckEvent
        {
            Event<T>.Register(obj);
        }

        public static void Unregister<T>(Action<T> obj) where T : IDeckEvent
        {
            Event<T>.Unregister(obj);
        }

        public static void Send<T>(T data) where T : IDeckEvent
        {
            Event<T>.Send(data);
        }

        public static void ClearEvents<T>() where T : IDeckEvent
        {
            Event<T>.Clear();
        }
    }

    public class Event<T> where T : IDeckEvent
    {
        private static Action<T> _listener;

        public static void Register(Action<T> obj)
        {
            _listener += obj;
        }

        public static void Unregister(Action<T> obj)
        {
            _listener -= obj;
        }

        public static void Send(T data)
        {
            _listener?.Invoke(data);
        }

        public static void Clear()
        {
            _listener = null;
        }
    }

    public interface IDeckEvent
    {
    }
}