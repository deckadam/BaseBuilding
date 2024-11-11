using DG.Tweening;
using UnityEngine;

namespace Deck.Utility.Animators
{
    [CreateAssetMenu(menuName = "Deck/Data/Animation/Deck Animation Parameters Animation Curve", fileName = "Deck Animation Parameters Animation Curve")]
    public class DeckAnimationParametersAnimationCurve : ScriptableObject
    {
        [SerializeField] private bool isLocal;
        [SerializeField] private AnimationCurve value;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        public bool IsLocal => isLocal;

        public AnimationCurve Value => value;

        public float Duration => duration;

        public Ease Ease => ease;
    }
}