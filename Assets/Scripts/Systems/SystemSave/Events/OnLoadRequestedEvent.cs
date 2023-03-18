using Deck.EventManager;
using Deck.Save;

namespace Deck.Utility.Constants.SaveListingMenu.Events
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