using Cysharp.Threading.Tasks;
using Deck.Component;

namespace Deck.Components.Operations
{
    public class DeckCommandRelease : DeckCommand
    {
        private DeckAgent _target;
        
        public DeckCommandRelease(DeckAgent target)
        {
            _target = target;
            commandType = DeckCommandType.Release;
        }

        public override UniTask<bool> ProcessCommand()
        {
            foreach (var deckComponent in _target.GetDeckComponents<DeckComponent>())
            {
                deckComponent.Release();
            }

            return base.ProcessCommand();
        }
    }
}