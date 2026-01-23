using EventManager;

namespace Services.Timing.Events
{
    public class DeckEventOnUpdate : IDeckEvent
    {
        public float deltaTime { get; private set; }

        public static DeckEventOnUpdate Create(float deltaTime)
        {
            return new DeckEventOnUpdate()
            {
                deltaTime = deltaTime
            };
        }
    }
}