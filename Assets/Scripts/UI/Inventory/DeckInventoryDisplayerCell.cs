using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Data.Item;
using Deck.Services.Implementations;
using Deck.Utility.Constants.GamePlay;
using Services.Implementations.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.Utility.Constants.Inventory
{
    public class DeckInventoryDisplayerCell : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private readonly Vector2 _centeredAnchor = Vector2.one / 2f;

        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI amount;
        [SerializeField] private RectTransform visualParent;

        private IMemoryPool _pool;
        private DeckInventoryPopUp _popup;
        private CancellationTokenSource _source;
        private DeckDataItem _item;
        private bool _isClicked;

        public void Initialize(DeckDataItem dataItem, DeckInventoryPopUp popup)
        {
            _item = dataItem;
            image.sprite = _item.GetIcon();
            amount.text = _item.GetAmount().ToString();
            _popup = popup;
        }

        public void Despawn()
        {
            _pool?.Despawn(this);
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
        }

        public void OnPointerDown()
        {
            if (_isClicked)
            {
                return;
            }

            _isClicked = true;
            _source?.Cancel();
            _source = new CancellationTokenSource();
            FollowCursor(_source);
        }

        public void OnPointerUp()
        {
            if (!_isClicked)
            {
                return;
            }

            _isClicked = false;
            _source?.Cancel();
        }

        private async void FollowCursor(CancellationTokenSource tokenSource)
        {
            visualParent.anchorMin = Vector2.zero;
            visualParent.anchorMax = Vector2.zero;
            visualParent.SetParent(Deck.GetService<DeckUIService>().GetUI<DeckGamePlayUI>().GetRectTransform());
            Deck.GetService<DeckInventoryService>().OnDragBegin(this);
            image.raycastTarget = false;
            while (!tokenSource.IsCancellationRequested)
            {
                visualParent.anchoredPosition = Input.mousePosition / 2f;
                await UniTask.NextFrame();
            }

            var isPlaced = Deck.GetService<DeckInventoryService>().TryToPlace();


            image.raycastTarget = true;
            visualParent.SetParent(transform, false);
            visualParent.anchorMin = _centeredAnchor;
            visualParent.anchorMax = _centeredAnchor;
            visualParent.anchoredPosition = Vector2.zero;

            if (isPlaced)
            {
                Despawn();
            }
        }

        public DeckInventoryPopUp GetPopUp() => _popup;
        public DeckDataItem GetItem() => _item;

        public class Factory : PlaceholderFactory<DeckInventoryDisplayerCell>
        {
        }
    }
}