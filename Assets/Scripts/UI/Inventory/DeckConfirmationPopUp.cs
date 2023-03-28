using System;
using Deck.Utility.Logger;
using TMPro;
using UnityEngine;
using Zenject;

namespace Deck.UI.Inventory
{
    public class DeckConfirmationPopUp : DeckPopUpBase, IPoolable<IMemoryPool>
    {
        [SerializeField] private TextMeshProUGUI display;
        private IMemoryPool _pool;
        private Action _onAccept;
        private Action _onDecline;

        public void Initialize(string text, Action onAccept, Action onDecline)
        {
            display.text = text;
            _onAccept = onAccept;
            _onDecline = onDecline;
        }

        public void OnAcceptClicked()
        {
            DeckLogger.UI("Confirmation dialogue accepted");
            _onAccept?.Invoke();
            _pool.Despawn(this);
        }

        public void OnDeclineClicked()
        {
            DeckLogger.UI("Confirmation dialogue canceled");
            _onDecline?.Invoke();
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
        }

        public class Factory : PlaceholderFactory<DeckConfirmationPopUp>
        {
        }

        public override bool CanDrag { get; } = false;
    }
}