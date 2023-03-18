using Cysharp.Threading.Tasks;
using Deck.Agent;

namespace Deck.Components
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

            return default;
        }
    }
}