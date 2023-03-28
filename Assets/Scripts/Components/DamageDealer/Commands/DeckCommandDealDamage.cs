using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Utility;

namespace Deck.Components
{
    public class DeckCommandDealDamage : DeckCommand
    {
        private bool _canKill;
        private DeckComponentDamageDealer _from;
        private DeckComponentHealth _to;
        private int _damage;
        private float _range;
        private Action _onAttackStart;
        private bool _continous;

        public DeckCommandDealDamage(int damage, float range, DeckComponentDamageDealer from, DeckComponentHealth to, Action onAttackStart = null, bool canKill = true, bool continuous = true)
        {
            _damage = damage;
            _range = range;
            _canKill = canKill;
            _to = to;
            _onAttackStart = onAttackStart;
            _from = from;
            _continous = continuous;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var movementComponent = _from.GetComponentHolder().GetDeckComponent<DeckComponentMovement>();
            if (_to.GetComponentHolder() == null)
            {
                return false;
            }

            var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(token, _to.GetComponentHolder().GetCancellationTokenOnDestroy()).Token;

            if (!_continous)
            {
                return await ExecuteDamageDealing(linkedToken, movementComponent);
            }

            while (!linkedToken.IsCancellationRequested)
            {
                var result = await ExecuteDamageDealing(linkedToken, movementComponent);
                if (!result)
                {
                    return false;
                }

                await UniTask.WaitWhile(() => !_from.CanAttack(), PlayerLoopTiming.Update, linkedToken);
            }

            return true;
        }

        private async Task<bool> ExecuteDamageDealing(CancellationToken token, DeckComponentMovement movementComponent)
        {
            var result = await DeckCommandUtility.AwaitTillDestinationIsReached(movementComponent, _to.GetComponentHolder().transform, _range, token);
            if (!result)
            {
                return false;
            }

            _onAttackStart?.Invoke();
            _from.OnAttack();
            _to.ReduceHealth(_damage, _canKill);
            return true;
        }

        public override bool InterrupintgCommand()
        {
            return true;
        }
    }
}