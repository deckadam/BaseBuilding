using DG.Tweening;
using UnityEngine;

namespace Deck.Utility.Animators
{
    [CreateAssetMenu(menuName = "Deck/Data/Animation/Deck Animation Parameters Float", fileName = "Deck Animation Parameters Float")]
    public class DeckAnimationParametersFloat : ScriptableObject
    {
        [SerializeField] private bool isLocal;
        [SerializeField] private float amount;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        public bool IsLocal => isLocal;

        public float Amount => amount;

        public float Duration => duration;

        public Ease Ease => ease;
    }
}