using EventManager;

namespace Services.PathFinding.Events
{
    public class DeckEventOnPathStatusChanged : IDeckEvent
    {
        public bool status { get; private set; }

        public static DeckEventOnPathStatusChanged Create(bool status)
        {
            return new DeckEventOnPathStatusChanged()
            {
                status = status
            };
        }
    }
}