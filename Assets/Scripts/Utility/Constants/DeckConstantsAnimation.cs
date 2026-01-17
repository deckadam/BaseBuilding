using UnityEngine;

namespace Utility.Constants
{
    public static class DeckConstantsAnimation
    {
        public static readonly int MovementSpeed = Animator.StringToHash("MovementSpeed");
        public static readonly int HumanoidAttack = Animator.StringToHash("HumanoidAttack");
        public static readonly int GetHit = Animator.StringToHash("GetHit");
        public static readonly int Sit = Animator.StringToHash("Sit");
        public static readonly int GetUp = Animator.StringToHash("GetUp");
    }
}