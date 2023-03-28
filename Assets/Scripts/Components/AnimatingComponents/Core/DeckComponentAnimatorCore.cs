using System;
using Deck.Components;
using Deck.Constants;
using UnityEngine;

namespace Deck.Components
{
    public class DeckComponentAnimatorCore : DeckComponent, IDeckAnimationImmediatePlay, IDeckAnimationSetBool, IDeckAnimationSetFloat
    {
        private DeckComponentMovement _movementComponent;
        private DeckComponentDamageDealer _damageDealerComponent;
        private Animator _animator;

        protected override void InternalPostInitialize()
        {
            _movementComponent = holder.GetDeckComponent<DeckComponentMovement>();
            if (_movementComponent == null)
            {
                throw new Exception("Movement component couldn't be found");
            }

            _damageDealerComponent = holder.GetDeckComponent<DeckComponentDamageDealer>();
            if (_damageDealerComponent == null)
            {
                throw new Exception("Damage dealer component couldn't be found");
            }

            _damageDealerComponent.OnDamageDealRequested += OnAttack;

            _animator = holder.GetComponentInChildren<Animator>();

            if (_animator == null)
            {
                throw new Exception("Animator couldn't be found");
            }
        }

        public override void DeInitialize()
        {
            _damageDealerComponent.OnDamageDealRequested -= OnAttack;
        }

        private void OnAttack(int duration)
        {
            _movementComponent.InterruptMovement(duration);
            _animator.SetTrigger(DeckConstantsAnimator.HumanoidAttack);
        }

        public override void Tick()
        {
            _animator.SetFloat(DeckConstantsAnimator.MovementSpeed, _movementComponent.GetSpeed());
        }

        public void Animate(string name)
        {
            _animator.Play(name);
        }

        public void Animate(string name, bool value)
        {
            _animator.SetBool(name, value);
        }

        public void Animate(string name, float value)
        {
            _animator.SetFloat(name, value);
        }
    }
}