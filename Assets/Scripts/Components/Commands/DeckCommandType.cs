using System;
using Cysharp.Threading.Tasks;

namespace Deck.Components.Operations
{
    public class DeckCommand
    {
        public DeckCommandType commandType;

        public virtual UniTask<bool> ProcessCommand()
        {
            return default;
        }
    }

    public enum DeckCommandType
    {
        Select,
        Possess,
        Release,
        Movement,
        TakeDamage,
        DealDamage,
        AddItem,
        RemoveItem
    }
}