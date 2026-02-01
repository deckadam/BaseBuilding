using System;

namespace EventManager
{
    public static class DeckEventManager
    {
        public static void Register<T>(Action<T> obj, DeckEventPriority eventPriority = DeckEventPriority.Low) where T : IDeckEvent
        {
            Event<T>.Register(obj, eventPriority);
        }

        public static void Unregister<T>(Action<T> obj, DeckEventPriority eventPriority = DeckEventPriority.Low) where T : IDeckEvent
        {
            Event<T>.Unregister(obj, eventPriority);
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
        private static Action<T> _lowListener;
        private static Action<T> _mediumListener;
        private static Action<T> _highListener;
        private static Action<T> _criticalListener;

        public static void Register(Action<T> obj, DeckEventPriority eventPriority)
        {
            switch (eventPriority)
            {
                case DeckEventPriority.Low:
                    _lowListener += obj;
                    break;
                case DeckEventPriority.Medium:
                    _mediumListener += obj;
                    break;
                case DeckEventPriority.High:
                    _highListener += obj;
                    break;
                case DeckEventPriority.Critical:
                    _criticalListener += obj;
                    break;
                default:
                    throw new Exception("Unknown deck event priority");
            }
        }

        public static void Unregister(Action<T> obj, DeckEventPriority eventPriority)
        {
            switch (eventPriority)
            {
                case DeckEventPriority.Low:
                    _lowListener -= obj;
                    break;
                case DeckEventPriority.Medium:
                    _mediumListener -= obj;
                    break;
                case DeckEventPriority.High:
                    _highListener -= obj;
                    break;
                case DeckEventPriority.Critical:
                    _criticalListener -= obj;
                    break;
                default:
                    throw new Exception("Unknown deck event priority");
            }
        }

        public static void Send(T data)
        {
            _criticalListener?.Invoke(data);
            _highListener?.Invoke(data);
            _mediumListener?.Invoke(data);
            _lowListener?.Invoke(data);
        }

        public static void Clear()
        {
            _lowListener = null;
            _mediumListener = null;
            _highListener = null;
            _criticalListener = null;
        }
    }

    public interface IDeckEvent
    {
    }
}

public enum DeckEventPriority
{
    Low,
    Medium,
    High,
    Critical
}