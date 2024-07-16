using Cysharp.Threading.Tasks;
using Deck.Utility.Poolable;
using DG.Tweening;
using UnityEngine;

namespace Deck.UI.Building
{
    public class DeckBuildingPage : DeckUIElement
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float appearDuration;
        [SerializeField] private Ease ease;

        protected DeckBuildingUI buildingUI;
        protected DeckBuildingButton button;

        public void Initialize(DeckBuildingUI buildingUI, DeckBuildingButton button)
        {
            this.buildingUI = buildingUI;
            this.button = button;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        public async UniTask Appear()
        {
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            await canvasGroup.DOFade(1f, appearDuration).SetEase(ease).AsyncWaitForCompletion();
        }

        public async UniTask Disappear()
        {
            canvasGroup.interactable = false;
            await canvasGroup.DOFade(0f, appearDuration).SetEase(ease).AsyncWaitForCompletion();
            gameObject.SetActive(false);
        }
    }
}