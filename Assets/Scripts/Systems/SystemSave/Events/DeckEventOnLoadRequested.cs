using Deck.EventManager;
using Deck.Save;

namespace Deck.Components.Building.SaveListingMenu.Events
{
    public class DeckEventOnLoadRequested : IDeckEvent
    {
        public DeckSaveSystem.SaveFile saveFile { get; private set; }

        public static DeckEventOnLoadRequested Create(DeckSaveSystem.SaveFile path)
        {
            return new()
            {
                saveFile = path
            };
        }
    }
}