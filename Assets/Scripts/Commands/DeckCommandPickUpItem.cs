using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Item;
using Deck.Save;
using Deck.Utility;
using Services.AgentFinder;

namespace Deck.Commands
{
    public class DeckCommandPickUpItem : DeckCommand
    {
        private DeckComponentInventory _inventory;
        private DeckComponentMovement _movement;
        private DeckItemVisual _itemVisual;

        public void Initialize(DeckComponentInventory inventory, DeckComponentMovement movement, DeckItemVisual itemVisual)
        {
            _inventory = inventory;
            _movement = movement;
            _itemVisual = itemVisual;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var isCanceled = await DeckCommandUtility.AwaitTillDestinationIsReached(_movement, _itemVisual.transform, 2f, token);
            if (isCanceled)
            {
                return false;
            }

            _itemVisual.OnPickUp(_movement.GetComponentHolder().GetCenter());
            _inventory.AddItem(_itemVisual.GetBindedItem());
            return true;
        }

        public override string GetSaveData()
        {
            var data = new SaveData(_inventory, _itemVisual);
            return DeckSaveUtility.GetSerializedData(data);
        }

        public override void LoadSaveData(string saveData)
        {
            var data = DeckSaveUtility.GetDeserializedData<SaveData>(saveData);
            var agent = Deck.GetService<DeckServiceFinder>().GetAgent(data.targetAgentId);
            _itemVisual = Deck.GetService<DeckServiceFinder>().GetItemVisual(data.itemId);
            _inventory = agent.GetDeckComponent<DeckComponentInventory>();
            _movement = agent.GetDeckComponent<DeckComponentMovement>();
        }

        [Serializable]
        private struct SaveData
        {
            public string targetAgentId;
            public string itemId;

            public SaveData(DeckComponentInventory target, DeckItemVisual itemVisual)
            {
                targetAgentId = target.GetComponentHolder().GetUniqueId().ID.ToString();
                itemId = itemVisual.UniqueId.ToString();
            }
        }
    }
}