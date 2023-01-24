using Deck.Component;
using Deck.EventManager;

namespace Deck.Services.Implementations.CellSelectionService.Events
{
    public class DeckOnComponentHolderDeath : DeckEvent
    {
        public DeckComponentHolder componentHolder { get; private set; }

        public static DeckOnComponentHolderDeath Create(DeckComponentHolder componentHolder)
        {
            return new DeckOnComponentHolderDeath()
            {
                componentHolder = componentHolder
            };
        }
    }
}