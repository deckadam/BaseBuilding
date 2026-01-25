using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnCameraRotateWithKeyboardStateChanged : IDeckEvent
    {
        public bool newState { get; private set; }

        public static DeckEventOnCameraRotateWithKeyboardStateChanged Create(bool newState)
        {
            return new DeckEventOnCameraRotateWithKeyboardStateChanged
            {
                newState = newState
            };
        }
    }
}