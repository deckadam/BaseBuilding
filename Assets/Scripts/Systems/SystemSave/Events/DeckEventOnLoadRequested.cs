using Deck.EventManager;
using Deck.Save;

namespace Deck.UI.SaveListingMenu.Events
{
    public class DeckEventOnLoadRequested : DeckEvent
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