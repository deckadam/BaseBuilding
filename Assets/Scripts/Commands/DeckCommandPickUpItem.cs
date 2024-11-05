using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Save;
using Deck.Utility;
using Services.AgentFinder;

namespace Deck.Components
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

            _itemVisual.OnPickUp(_movement.GetAgent().GetCenter());
            _inventory.AddItem(_itemVisual.GetBoundItem());
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
            _itemVisual = Deck.GetService<DeckServiceFinder>().GetItemVisual(data.prefabId);
            _inventory = agent.GetDeckComponent<DeckComponentInventory>();
            _movement = agent.GetDeckComponent<DeckComponentMovement>();
        }

        [Serializable]
        private struct SaveData
        {
            public int targetAgentId;
            public int prefabId;

            public SaveData(DeckComponentInventory target, DeckItemVisual itemVisual)
            {
                targetAgentId = target.GetAgent().GetUniqueId().ID;
                prefabId = itemVisual.UniqueId.ID;
            }
        }
    }
}