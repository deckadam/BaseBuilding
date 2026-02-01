using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnMiddleScroll:IDeckEvent
    {
        public float scrollValue { get; private set; }
        
        public static DeckEventOnMiddleScroll Create(float scrollValue)
        {
            return new DeckEventOnMiddleScroll
            {
                scrollValue = scrollValue
            };
        }
    }
}