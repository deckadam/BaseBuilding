using Deck.EventManager;

namespace Deck.InputHandling.Events
{
    public class DeckEventMiddleScroll:IDeckEvent
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