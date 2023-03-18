using Cysharp.Threading.Tasks;

namespace Deck.Components
{
    public class DeckCommandSelect : DeckCommand
    {
        public DeckCommandSelect()
        {
            commandType = DeckCommandType.Select;
        }
    }
}