using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;

namespace Deck.Commands.Sit
{
    public class DeckCommandSit : DeckCommand
    {
        private DeckAgent _agent;
        private DeckAgent _target;
        
        public DeckCommandSit(DeckAgent agent,DeckAgent target)
        {
            _agent = agent;
            _target = target;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_target.transform.position, _agent).ProcessCommand(token);
            _agent.GetDeckComponent<DeckComponentMovement>().SetRotation(_target.transform.rotation);
            _agent.transform.rotation = _target.transform.rotation;
            var animatorHumanoid = _agent.GetDeckComponent<IDeckAnimationSetTrigger>();
            animatorHumanoid.Trigger("Sit");
            await UniTask.Delay(1000, cancellationToken: token);
            return true;
        }
    }
}