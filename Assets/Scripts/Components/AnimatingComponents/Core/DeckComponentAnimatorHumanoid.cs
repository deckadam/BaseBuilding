using System;
using Base;
using Components.AnimatingComponents.Interfaces;
using Components.Movement;
using UnityEngine;
using Utility.Constants;

namespace Components.AnimatingComponents.Core
{
    public class DeckComponentAnimatorHumanoid : DeckComponent, IDeckAnimationImmediatePlay, IDeckAnimationSetBool, IDeckAnimationSetFloat,IDeckAnimationSetTrigger
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
            _animator.SetFloat(DeckConstantsAnimation.MovementSpeed, _movementComponent.GetSpeed());
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

        public void Trigger(int id)
        {
            _animator.SetTrigger(id);
        }
    }
}