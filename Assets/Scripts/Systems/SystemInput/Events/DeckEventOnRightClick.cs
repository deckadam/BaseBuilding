using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnRightClick : IDeckEvent
    {
        public static DeckEventOnRightClick Create()
        {
            return new DeckEventOnRightClick();
        }
    }
}