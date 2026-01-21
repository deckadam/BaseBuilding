using EventManager;

namespace Systems.SystemSave.Events
{
    public struct DeckEventOnLoadRequested : IDeckEvent
    {
        public DeckSaveSystem.SaveFile saveFile { get; private set; }

        public static DeckEventOnLoadRequested Create(DeckSaveSystem.SaveFile path)
        {
            return new DeckEventOnLoadRequested
            {
                saveFile = path
            };
        }
    }
}