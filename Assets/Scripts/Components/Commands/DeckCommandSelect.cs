using Cysharp.Threading.Tasks;

namespace Deck.Components.Operations
{
    public class DeckCommandSelect : DeckCommand
    {
        public DeckCommandSelect()
        {
            commandType = DeckCommandType.Select;
        }
    }
}