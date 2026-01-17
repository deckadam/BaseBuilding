using System;
using UnityEngine;

namespace Utility.Animators
{
    [Serializable]
    public class DeckAnimator
    {
        [SerializeField] protected Transform target;
        [SerializeField] protected DeckAnimationParametersVector parameters;

        public DeckAnimationParametersVector GetParameters() => parameters;
        public Transform GetTarget() => target;

        public virtual void Animate()
        {
        }

        public virtual void Kill()
        {
        }
    }
}