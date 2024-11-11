using System;
using Deck.Utility.Logger;
using TMPro;
using UnityEngine;

namespace Deck.UI.Confirmation
{
    public class DeckConfirmationPopUp : DeckPopUpBase
    {
        [SerializeField] private TextMeshProUGUI display;
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
            OnCloseRequested();
        }

        public void OnDeclineClicked()
        {
            DeckLogger.UI("Confirmation dialogue canceled");
            _onDecline?.Invoke();
            OnCloseRequested();
        }

        public override bool CanDrag => false;
    }
}