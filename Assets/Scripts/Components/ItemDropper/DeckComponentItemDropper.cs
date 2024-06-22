using Deck.Data.ItemDrop;
using Deck.ItemVisualProviders;
using Deck.Utility.Logger;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deck.Commands
{
    public class DeckComponentItemDropper : DeckComponent
    {
        [SerializeField] private DeckDataItemDrop dropData;

        protected override void InternalPreInitialize()
        {
            if (dropData == null)
            {
                DeckLogger.Error("Item drop data not found");
            }
        }

        public override void OnDeath()
        {
            var rolledItems = dropData.RollItem();
            foreach (var rolledItem in rolledItems)
            {
                DropItem(rolledItem);
            }
        }

        private void DropItem((DeckDataItemDrop.ItemDrop itemDrop, int amount) drop)
        {
            var item = drop.itemDrop.GetItem();
            for (var i = 0; i < drop.amount; i++)
            {
                var createdItem = Deck.GetService<DeckServiceItemVisual>().RentItemVisual(item.Representation.PrefabId);
                createdItem.transform.position = agent.transform.position;
                createdItem.transform.rotation = Random.rotation;
                createdItem.OnDroppped();
                createdItem.ThrowInRandomDirection();
            }
        }
    }
}