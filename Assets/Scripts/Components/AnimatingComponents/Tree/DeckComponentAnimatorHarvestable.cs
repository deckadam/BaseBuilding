using Base;
using Components.Health;
using DG.Tweening;
using UnityEngine;

namespace Components.AnimatingComponents.Tree
{
    public class DeckComponentAnimatorHarvestable : DeckComponent
    {
        // [SerializeField] private DeckDataAnimationTreeShake shakeDataAnimation;
        private Transform _cachedHolderTransform;
        private Tween _activeTween;

        protected override void InternalPostInitialize()
        {
            _cachedHolderTransform = agent.transform;
            agent.GetDeckComponent<DeckComponentHealth>().OnDamageTaken += OnOnDamageTaken;
        }

        public override void DeInitialize()
        {
            _activeTween?.Kill();
            agent.GetDeckComponent<DeckComponentHealth>().OnDamageTaken -= OnOnDamageTaken;
        }


        private void OnOnDamageTaken(DeckAgent damageDealer)
        {
            _activeTween?.Kill();
            // _activeTween = _cachedHolderTransform.DOShakeRotation(shakeDataAnimation.GetShakeAnimationDuration(), shakeDataAnimation.GetShakeAnimationStrength(), shakeDataAnimation.GetShakeAnimationVibrato()).SetEase(shakeDataAnimation.GetShakeAnimationEase());
        }
    }
}