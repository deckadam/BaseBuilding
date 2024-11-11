using Deck.Components;
using Deck.Services;
using Deck.UI.GamePlay;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck.UI
{
    [RequireComponent(typeof(EventTrigger))]
    public abstract class DeckPopUpBase : DeckUIElement
    {
        [SerializeField] protected RectTransform rect;

        private Vector2 _clickPosition;
        private Vector2 _startPosition;

        private void OnValidate()
        {
            rect = GetComponent<RectTransform>();
        }

        private void Awake()
        {
            var eventTrigger = GetComponent<EventTrigger>();

            var onPointerClick = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            onPointerClick.callback.AddListener(data => OnPointerDown(data as PointerEventData));
            var onPointerDrag = new EventTrigger.Entry { eventID = EventTriggerType.Drag };
            onPointerDrag.callback.AddListener(data => OnPointerDrag(data as PointerEventData));

            eventTrigger.triggers.Add(onPointerClick);
            eventTrigger.triggers.Add(onPointerDrag);
        }

        private void OnPointerDown(PointerEventData data)
        {
            _clickPosition = Input.mousePosition;
            _startPosition = rect.anchoredPosition;
        }

        private void OnPointerDrag(PointerEventData data)
        {
            if (!CanDrag)
            {
                return;
            }

            var delta = (Vector2)Input.mousePosition - _clickPosition;
            rect.anchoredPosition = delta + _startPosition;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            var gamePlayUIRect = Deck.GetService<DeckServiceUI>().GetUI<DeckUIGamePlay>().GetRectTransform();
            rect.SetParent(gamePlayUIRect, false);
            rect.anchoredPosition = Vector2.zero;
        }


        public void OnCloseRequested()
        {
            InstanceProvider.ReturnUIElement(this);
        }

        public virtual bool CanDrag => true;
    }
}