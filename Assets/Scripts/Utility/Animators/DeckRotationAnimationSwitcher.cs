using Animators;
using UnityEngine;

namespace Unility.Animators
{
    public class DeckRotationAnimationSwitcher : MonoBehaviour
    {
        [SerializeField] private DeckRotationAnimator item1;
        [SerializeField] private DeckRotationAnimator item2;

        [SerializeField] private string deckTag;

        public string GetTag() => deckTag;

        private void Awake()
        {
            item1.Initialize();
            item2.Initialize();
        }

        public void Animate(bool state, bool killOtherOnSwitch = true)
        {
            if (state)
            {
                if (killOtherOnSwitch)
                {
                    item2.Kill();
                }

                item1.Animate();
            }
            else
            {
                if (killOtherOnSwitch)
                {
                    item1.Kill();
                }

                item2.Animate();
            }
        }
    }
}