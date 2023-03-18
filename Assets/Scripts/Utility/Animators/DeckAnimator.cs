using System;
using UnityEngine;
using Deck.Utility.Animators;

namespace Animators
{
    [Serializable]
    public class DeckAnimator
    {
        [SerializeField] protected Transform target;
        [SerializeField] protected DeckAnimationParameters parameters;

        public DeckAnimationParameters GetParameters() => parameters;
        public Transform GetTarget() => target;

        public virtual void Animate()
        {
        }

        public virtual void Kill()
        {
        }
    }
}