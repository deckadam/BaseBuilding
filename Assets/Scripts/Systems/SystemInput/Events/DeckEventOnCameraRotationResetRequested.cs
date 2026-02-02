using EventManager;

namespace Systems.SystemInput.Events
{
    public struct DeckEventOnCameraRotationResetRequested : IDeckEvent
    {
        public static DeckEventOnCameraRotationResetRequested Create()
        {
            return new DeckEventOnCameraRotationResetRequested();
        }
    }
}