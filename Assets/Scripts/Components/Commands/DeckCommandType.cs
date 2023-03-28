using System.Threading;
using Cysharp.Threading.Tasks;

namespace Deck.Components
{
    public class DeckCommand
    {
        public virtual UniTask<bool> ProcessCommand(CancellationToken token)
        {
            return default;
        }

        public virtual bool InterrupintgCommand()
        {
            return false;
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