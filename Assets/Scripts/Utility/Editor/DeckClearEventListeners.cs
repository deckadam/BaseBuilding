using Deck.EventManager;
using Deck.InputHandling.Events;
using UnityEditor;

namespace Utility.Editor
{
    public static class DeckClearEventListeners
    {
        [MenuItem("Deck/Clear Listeners")]
        private static void ClearListeners()
        {
            DeckEventManager.ClearEvents<DeckOnNavMeshPositionSelection>();
        }
    }
}