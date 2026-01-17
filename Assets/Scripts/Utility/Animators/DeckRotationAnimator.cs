using System;
using DG.Tweening;

namespace Utility.Animators
{
    [Serializable]
    public class DeckRotationAnimator : DeckAnimator
    {
        private Action onCall;
        private Tween _tween;

        public void Initialize()
        {
            if (parameters.IsLocal)
            {
                onCall = () => { _tween = target.DOLocalRotate(parameters.Amount, parameters.Duration).SetEase(parameters.Ease); };
            }
            else
            {
                onCall = () => { _tween = target.DORotate(parameters.Amount, parameters.Duration).SetEase(parameters.Ease); };
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