using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnCameraRotateWithKeyboard : IDeckEvent
    {
        public bool direction { get; private set; }

        public static DeckEventOnCameraRotateWithKeyboard Create(bool direction)
        {
            return new DeckEventOnCameraRotateWithKeyboard()
            {
                direction = direction
            };
        }
    }
}