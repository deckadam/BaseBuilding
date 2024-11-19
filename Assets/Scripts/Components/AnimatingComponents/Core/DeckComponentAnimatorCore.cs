using System;
using Deck.Utility.Constants;
using UnityEngine;

namespace Deck.Components
{
    public class DeckComponentAnimatorCore : DeckComponent, IDeckAnimationImmediatePlay, IDeckAnimationSetBool, IDeckAnimationSetFloat
    {
        private DeckComponentMovement _movementComponent;
        private DeckComponentDamageDealer _damageDealerComponent;
        private DeckComponentHealth _healthComponent;
        private Animator _animator;

        protected override void InternalPostInitialize()
        {
            _movementComponent = agent.GetDeckComponent<DeckComponentMovement>();
            if (_movementComponent == null)
            {
                throw new Exception("Movement component couldn't be found");
            }

            _damageDealerComponent = agent.GetDeckComponent<DeckComponentDamageDealer>();
            if (_damageDealerComponent == null)
            {
                throw new Exception("Damage dealer component couldn't be found");
            }

            _damageDealerComponent.OnDamageDealRequested += OnAttack;

            _animator = agent.GetComponentInChildren<Animator>();

            if (_animator == null)
            {
                throw new Exception("Animator couldn't be found");
            }

            _healthComponent = agent.GetDeckComponent<DeckComponentHealth>();
            if (_healthComponent == null)
            {
                throw new Exception("Health component couldn't be found");
            }

            _healthComponent.OnDamageTaken += OnHealthChange;
        }

        public override void DeInitialize()
        {
            _damageDealerComponent.OnDamageDealRequested -= OnAttack;
            _healthComponent.OnDamageTaken -= OnHealthChange;
        }

        private void OnHealthChange(DeckAgent damageDealer)
        {
            _animator.SetTrigger(DeckConstantsAnimator.GetHit);
        }

        private void OnAttack(int duration)
        {
            _movementComponent.InterruptMovement(duration);
            _animator.SetTrigger(DeckConstantsAnimator.HumanoidAttack);
        }

        private void Update()
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