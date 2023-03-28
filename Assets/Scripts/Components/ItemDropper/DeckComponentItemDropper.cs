using System;
using Deck.Components;
using Deck.Item;
using Deck.Data.ItemDrop;
using Zenject;
using Random = UnityEngine.Random;

namespace Deck.Components
{
    public class DeckComponentItemDropper : DeckComponent
    {
        private DeckDataItemDrop dropData;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container)
        {
            _container = container;
        }

        protected override void InternalPreInitialize()
        {
            dropData = holder.GetData<DeckDataItemDrop>();
            if (dropData == null)
            {
                throw new Exception("Item drop data not found");
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
                var createdItem = _container.InstantiatePrefab(item.Representation).GetComponent<DeckItemVisual>();
                createdItem.transform.position = holder.transform.position;
                createdItem.transform.rotation = Random.rotation;
                createdItem.OnDroppped();
                createdItem.ThrowInRandomDirection();
            }
        }
    }
}