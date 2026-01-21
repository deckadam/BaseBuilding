using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventMiddleScroll:IDeckEvent
    {
        public float scrollValue { get; private set; }
        
        public static DeckEventMiddleScroll Create(float scrollValue)
        {
            return new DeckEventMiddleScroll
            {
                scrollValue = scrollValue
            };
        }
    }
}