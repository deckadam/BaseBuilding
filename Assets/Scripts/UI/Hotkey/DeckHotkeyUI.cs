using Deck.EventManager;
using Deck.UI.Hotkey.Events;

namespace Deck.UI.Hotkey
{
    public class DeckHotkeyUI : DeckUIBase
    {
        public DeckHotKeyPiece[] hotkeyObjects;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventOnHotkeySelected>(OnHotkeySelected);
            DeckEventManager.Register<DeckEventOnActiveHotkeyCountChanged>(OnHotkeyCountChanged);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnHotkeySelected>(OnHotkeySelected);
            DeckEventManager.Unregister<DeckEventOnActiveHotkeyCountChanged>(OnHotkeyCountChanged);
        }

        private void OnHotkeyCountChanged(DeckEventOnActiveHotkeyCountChanged obj)
        {
            for (var i = 0; i < obj.count; i++)
            {
                hotkeyObjects[i].gameObject.SetActive(true);
            }

            for (var i = obj.count; i < 10; i++)
            {
                hotkeyObjects[i].gameObject.SetActive(false);
            }
        }

        private void OnHotkeySelected(DeckEventOnHotkeySelected obj)
        {
            foreach (var deckHotKeyPiece in hotkeyObjects)
            {
                deckHotKeyPiece.Normalize();
            }

            hotkeyObjects[obj.index].Highlight();
        }

        protected override bool CanDisappear()
        {
            return false;
        }
    }
}