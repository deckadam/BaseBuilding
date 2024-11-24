using System;
using Deck.Utility.Constants;
using UnityEngine;

namespace Deck.Components
{
    public class DeckComponentAnimatorCore : DeckComponent, IDeckAnimationImmediatePlay, IDeckAnimationSetBool, IDeckAnimationSetFloat
    {
        private DeckComponentMovement _movementComponent;
        private Animator _animator;

        protected override void InternalPostInitialize()
        {
            _movementComponent = agent.GetDeckComponent<DeckComponentMovement>();
            if (_movementComponent == null)
            {
                throw new Exception("Movement component couldn't be found");
            }

            _animator = agent.GetComponentInChildren<Animator>();

            if (_animator == null)
            {
                throw new Exception("Animator couldn't be found");
            }
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