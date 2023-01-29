using System;

namespace Deck.Components.Operations
{
    public class DeckCommand
    {
        public DeckCommandType commandType;
    }

    public class DeckCommandListener
    {
        public DeckCommandType commandType { get; private set; }
        public Action<DeckCommand> listener { get; private set; }

        public static DeckCommandListener Create(DeckCommandType commandType, Action<DeckCommand> listener)
        {
            return new()
            {
                listener = listener,
                commandType = commandType
            };
        }
    }

    public enum DeckCommandType
    {
        Select,
        Possess,
        Release,
        Move,
        TakeDamage,
        DealDamage,
        AddItem,
        RemoveItem
    }
}