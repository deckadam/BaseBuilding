using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Deck.Utility;

namespace Deck.Commands
{
    public class DeckCommandDealDamage : DeckCommand
    {
        private bool _canKill;
        private DeckComponentBasicInteraction _from;
        private DeckComponentHealth _to;
        private int _damage;
        private float _baseAttackRange;
        private Action _onAttackStart;
        private bool _continous;

        public DeckCommandDealDamage(int damage, float baseAttackRange, DeckComponentBasicInteraction from, DeckComponentHealth to, Action onAttackStart = null, bool canKill = true, bool continuous = true)
        {
            _damage = damage;
            _baseAttackRange = baseAttackRange;
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

            if (!_continous)
            {
                return await ExecuteDamageDealing(token, movementComponent);
            }

            while (!token.IsCancellationRequested)
            {
                var result = await ExecuteDamageDealing(token, movementComponent);
                if (!result)
                {
                    return false;
                }

                var isCanceled = await UniTask.WaitWhile(() => !_from.CanAttack(), cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled)
                {
                    return false;
                }
            }

            return true;
        }

        private async UniTask<bool> ExecuteDamageDealing(CancellationToken token, DeckComponentMovement movementComponent)
        {
            if (_to.IsDead)
            {
                return false;
            }

            var range = _to.GetComponentHolder().GetSize() + _baseAttackRange;

            var isCanceled = await DeckCommandUtility.AwaitTillDestinationIsReached(movementComponent, _to.GetComponentHolder().transform, range, token);
            if (isCanceled)
            {
                return false;
            }

            if (_to.IsDead)
            {
                return false;
            }

            _onAttackStart?.Invoke();
            _from.OnAttack();

            _to.ReduceHealth(_from.GetComponentHolder(), _damage, _canKill);
            return true;
        }
    }
}