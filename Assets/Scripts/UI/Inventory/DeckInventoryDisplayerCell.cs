using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;
using Deck.Data.Item;
using Deck.ItemVisualProviders.Inventory;
using Deck.Services.Selection;
using Deck.Services.UI;
using Deck.UI.GamePlay;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Deck.UI.Inventory
{
    public class DeckInventoryDisplayerCell : DeckUIElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private RectTransform visualParent;

        private DeckInventoryPopUp _popup;
        private CancellationTokenSource _source;
        private DeckDataItem _item;
        private bool _isClicked;
        private bool _isHovering;
        private int _amount;

        public void Initialize(DeckDataItem dataItem, int amount, DeckInventoryPopUp popup)
        {
            _item = dataItem;
            image.sprite = _item.Icon;
            _amount = amount;
            amountText.text = amount.ToString();
            _popup = popup;
        }

        protected override void InternalOnDeSpawned()
        {
            _source?.Cancel();
            _source?.Dispose();
            _source = null;
        }

        private async void FollowCursor(CancellationTokenSource tokenSource)
        {
            visualParent.SetParent(Deck.GetService<DeckServiceUI>().GetUI<DeckUIGamePlay>().GetRectTransform(), true);
            Deck.GetService<DeckServiceInventory>().OnDragBegin(this);
            image.raycastTarget = false;
            var offset = new Vector2(Screen.width / 2f, Screen.height / 2f);
            while (!tokenSource.IsCancellationRequested)
            {
                visualParent.anchoredPosition = (Vector2)Input.mousePosition - offset;
                await UniTask.NextFrame();
            }

            Deck.GetService<DeckServiceInventory>().TryToPlace();

            image.raycastTarget = true;
            visualParent.SetParent(transform, true);
            visualParent.anchoredPosition = Vector2.zero;
        }

        public DeckInventoryPopUp GetPopUp() => _popup;
        public DeckDataItem GetItem() => _item;
        public int GetAmount() => _amount;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isClicked)
            {
                return;
            }

            _isClicked = true;
            _source?.Cancel();
            _source?.Dispose();
            _source = new CancellationTokenSource();
            FollowCursor(_source);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isClicked)
            {
                return;
            }

            _isClicked = false;
            _source?.Cancel();
            _source?.Dispose();
            _source = null;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHovering = false;
        }

        private void Update()
        {
            if (!_isHovering)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                DeckServiceSelection.currentPossession.GetDeckComponent<IDeckItemItemHolder>().SetItemToHold(_item);
            }
        }
    }
}