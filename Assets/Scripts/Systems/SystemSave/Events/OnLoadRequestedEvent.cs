using Deck.EventManager;
using Deck.Save;

namespace Deck.UI.SaveListingMenu.Events
{
    public class OnLoadRequestedEvent : DeckEvent
    {
        public DeckSaveSystem.SaveFile saveFile { get; private set; }

        public static OnLoadRequestedEvent Create(DeckSaveSystem.SaveFile path)
        {
            return new()
            {
                saveFile = path
            };
        }
    }
}