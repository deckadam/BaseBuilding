using Deck.EventManager;
using Deck.UI.Hotkey.Events;

namespace Deck.UI.Hotkey
{
    public class DeckHotkeyUI : DeckUIBase
    {
        public DeckHotKeyPiece[] hotkeyObjects;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckOnHotkeySelected>(OnHotkeySelected);
            DeckEventManager.Register<DeckOnActiveHotkeyCountChanged>(OnHotkeyCountChanged);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnHotkeySelected>(OnHotkeySelected);
            DeckEventManager.Unregister<DeckOnActiveHotkeyCountChanged>(OnHotkeyCountChanged);
        }

        private void OnHotkeyCountChanged(DeckOnActiveHotkeyCountChanged obj)
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

        private void OnHotkeySelected(DeckOnHotkeySelected obj)
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