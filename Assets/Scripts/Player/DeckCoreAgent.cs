using Deck.Generalnterfaces;
using Deck.Inventory.Inventory;
using UnityEngine;
using Zenject;

namespace Deck.Player
{
    public class DeckCoreAgent : MonoBehaviour, IDeckInventoryHolder
    {
        [Inject] private IDeckCorePlayerSystem movementHandler;
        [Inject] private DeckInventory inventory;

        private void OnEnable()
        {
            movementHandler.Initialize(this);
        }

        private void OnDisable()
        {
            movementHandler.DeInitialize();
        }

        public DeckInventory GetInventory() => inventory;
    }
}