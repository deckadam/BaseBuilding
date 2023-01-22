using Deck.EventManager;
using Deck.SaveService;

namespace Deck.UI.SaveListingMenu.Events
{
    public class OnLoadRequestedEvent : DeckEvent
    {
        public DeckSaveManager.SaveFile saveFile { get; private set; }

        public static OnLoadRequestedEvent Create(DeckSaveManager.SaveFile path)
        {
            return new()
            {
                saveFile = path
            };
        }
    }
}