using System;
using DG.Tweening;

namespace Animators
{
    [Serializable]
    public class DeckRotationAnimator : DeckAnimator
    {
        private Action onCall;
        private Tween _tween;

        public void Initialize()
        {
            if (parameters.isLocal)
            {
                onCall = () => { _tween = target.DOLocalRotate(parameters.amount, parameters.duration).SetEase(parameters.ease); };
            }
            else
            {
                onCall = () => { _tween = target.DORotate(parameters.amount, parameters.duration).SetEase(parameters.ease); };
            }
        }

        public override void Animate()
        {
            onCall();
        }


        public override void Kill()
        {
            _tween?.Kill();
            _tween = null;
        }
    }
}