using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnMiddleMouseButtonStatusChange : IDeckEvent
    {
        public bool status { get; private set; }

        public static DeckEventOnMiddleMouseButtonStatusChange Create(bool status)
        {
            return new DeckEventOnMiddleMouseButtonStatusChange()
            {
                status = status
            };
        }
    }
}