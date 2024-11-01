using Deck.EventManager;
using Deck.InputHandling.Events;
using UnityEditor;

namespace Deck.Utility.Editor
{
    public static class DeckClearEventListeners
    {
        [MenuItem("Deck/Clear Listeners")]
        private static void ClearListeners()
        {
            DeckEventManager.ClearEvents<DeckEventMiddleScroll>();
            DeckEventManager.ClearEvents<DeckEventOnLeftClickDown>();
            DeckEventManager.ClearEvents<DeckEventOnLeftClickUp>();
            DeckEventManager.ClearEvents<DeckEventOnMouseMove>();
        }
    }
}