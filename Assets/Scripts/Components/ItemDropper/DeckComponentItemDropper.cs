using Base;
using Data.Agent;
using Services;
using Services.ItemVisual;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Components.ItemDropper
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
                DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(item.Representation.PrefabId, out var createdItem);

                createdItem.transform.position = agent.transform.position;
                createdItem.transform.rotation = Random.rotation;
                createdItem.ThrowInRandomDirection();
            }
        }
    }
}