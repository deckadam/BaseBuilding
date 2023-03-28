using DG.Tweening;
using UnityEngine;

namespace Deck.Animators
{
    [CreateAssetMenu(menuName = "Deck/Data/Animation/Deck Animation Parameters Vector", fileName = "Deck Animation Parameters Vector")]
    public class DeckAnimationParametersVector : ScriptableObject
    {
        [SerializeField] private bool isLocal;
        [SerializeField] private Vector3 amount;
        [SerializeField] private float duration;
        [SerializeField] private Ease ease;

        public bool IsLocal => isLocal;

        public Vector3 Amount => amount;

        public float Duration => duration;

        public Ease Ease => ease;
    }
}