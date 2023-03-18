using System;
using DG.Tweening;
using UnityEngine;

namespace Deck.Utility.Animators
{
    [CreateAssetMenu(menuName = "Deck/Data/Animation", fileName = "Deck Animation Parameters")]
    public class DeckAnimationParameters : ScriptableObject
    {
        public bool isLocal;
        public Vector3 amount;
        public float duration;
        public Ease ease;
    }
}