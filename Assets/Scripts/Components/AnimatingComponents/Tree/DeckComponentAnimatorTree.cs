using Data.Component.Tree;
using Deck.Components;
using DG.Tweening;
using UnityEngine;

namespace Deck.Components
{
    public class DeckComponentAnimatorTree : DeckComponent
    {
        private Transform _cachedHolderTransform;
        private DeckDataAnimationTreeShake _shakeDataAnimation;
        private Tween _activeTween;

        protected override void InternalPostInitialize()
        {
            _cachedHolderTransform = holder.transform;
            _shakeDataAnimation = holder.GetData<DeckDataAnimationTreeShake>();
            holder.GetDeckComponent<DeckComponentHealth>().OnDamageTaken += OnOnDamageTaken;
        }

        public override void DeInitialize()
        {
            _activeTween?.Kill();
            holder.GetDeckComponent<DeckComponentHealth>().OnDamageTaken -= OnOnDamageTaken;
        }


        private void OnOnDamageTaken()
        {
            _activeTween?.Kill();
            _activeTween = _cachedHolderTransform.DOShakeRotation(_shakeDataAnimation.GetShakeAnimationDuration(), _shakeDataAnimation.GetShakeAnimationStrength(), _shakeDataAnimation.GetShakeAnimationVibrato()).SetEase(_shakeDataAnimation.GetShakeAnimationEase());
        }
    }
}