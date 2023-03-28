using DG.Tweening;
using UnityEngine;

namespace Data.Component.Tree
{
    [CreateAssetMenu(fileName = "Deck Data Animation Tree Shake", menuName = "Deck/Data/Component/Animation/Tree Shake")]
    public class DeckDataAnimationTreeShake : DeckDataComponent
    {
        [SerializeField] private float shakeAnimationDuration;
        [SerializeField] private float shakeAnimationStrength;
        [SerializeField] private int shakeAnimationVibrato;
        [SerializeField] private Ease shakeAnimationEase;

        public float GetShakeAnimationDuration() => shakeAnimationDuration;
        public float GetShakeAnimationStrength() => shakeAnimationStrength;
        public int GetShakeAnimationVibrato() => shakeAnimationVibrato;
        public Ease GetShakeAnimationEase() => shakeAnimationEase;
    }
}