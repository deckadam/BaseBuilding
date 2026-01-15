using Deck.Base;
using Deck.Data.ItemDrop;
using Deck.Utility;
using Services.ItemVisual;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Deck.Components
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

        public override void OnDestroy()
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
                if (!global::Services.DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(item.Representation.PrefabId, out var createdItem))
                {
                    DeckLogger.Error("Item visual not found for id " + item.Representation.PrefabId);
                    continue;
                }

                createdItem.transform.position = agent.transform.position;
                createdItem.transform.rotation = Random.rotation;
                createdItem.ThrowInRandomDirection();
            }
        }
    }
}