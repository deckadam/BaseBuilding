using Cysharp.Threading.Tasks;
using Deck.Data.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DeckUIBase : MonoBehaviour
    {
        [Inject] protected DeckUIData uiData;
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected bool isAppearedOnStartUp;

        [ShowInInspector, ReadOnly] internal bool _isAppeared;
        internal bool isAppearing;
        internal bool isDisappearing;

        private void OnValidate()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            _isAppeared = isAppearedOnStartUp;
            if (_isAppeared)
            {
                Appear();
            }
            else
            {
                Disappear();
            }
        }

        public void SwapAppearanceStatus()
        {
            if (_isAppeared)
            {
                Debug.LogError("Disappearing");
                Disappear();
            }
            else
            {
                Debug.LogError("Appearing");
                Appear();
            }
        }

        public async UniTask Appear()
        {
            isAppearing = true;
            canvasGroup.interactable = false;

            OnPreAppear();
            await canvasGroup.DOFade(1f, uiData.canvasAppearDuration).AsyncWaitForCompletion();

            _isAppeared = true;
            canvasGroup.interactable = true;
            isAppearing = false;

            OnPostAppear();
        }

        public async UniTask Disappear()
        {
            isDisappearing = true;
            canvasGroup.interactable = true;
            OnPreDisappear();
            await canvasGroup.DOFade(0f, uiData.canvasDisapearDuration).AsyncWaitForCompletion();
            _isAppeared = false;
            canvasGroup.interactable = false;
            isDisappearing = false;
            OnPostDisappear();
        }

        public virtual void OnPreAppear()
        {
        }

        public virtual void OnPostAppear()
        {
        }

        public virtual void OnPreDisappear()
        {
        }

        public virtual void OnPostDisappear()
        {
        }
    }
}