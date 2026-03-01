using System;
using System.Threading;
using Base;
using Cysharp.Threading.Tasks;
using InGame.Agent.Raid;
using Services;
using Services.Finder;
using Services.Raid;
using Systems.SystemSave;
using UnityEngine;

namespace Commands
{
    public class DeckCommandLootChest : DeckCommand
    {
        private DeckAgent _targetChest;
        private DeckAgentRaider _lootingAgent;
        private float _totalLootDuration;
        private float _currentLootDuration;

        public DeckCommandLootChest(DeckAgent targetChest, DeckAgentRaider lootingRaider, float totalLootDuration)
        {
            _targetChest = targetChest;
            _lootingAgent = lootingRaider;
            _totalLootDuration = totalLootDuration;
            _currentLootDuration = 0;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            while (_currentLootDuration < _totalLootDuration)
            {
                await UniTask.NextFrame(token);
                _currentLootDuration += Time.deltaTime;
            }

            DeckServiceProvider.GetService<DeckServiceRaid>().GetRaidController().OnChestLooted();

            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new DeckCommandLootChestSaveData(_targetChest.UniqueId.Id, _lootingAgent.UniqueId.Id, _totalLootDuration, _currentLootDuration));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<DeckCommandLootChestSaveData>(saveData);
            _targetChest = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(loadedData.targetChestId);
            _lootingAgent = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent<DeckAgentRaider>(loadedData.raiderId);
            _totalLootDuration = loadedData.totalLootDuration;
            _currentLootDuration = loadedData.currentLootDuration;
        }

        [Serializable]
        public struct DeckCommandLootChestSaveData
        {
            public int targetChestId;
            public int raiderId;
            public float totalLootDuration;
            public float currentLootDuration;

            public DeckCommandLootChestSaveData(int targetChestId, int raiderId, float totalLootDuration, float currentLootDuration)
            {
                this.targetChestId = targetChestId;
                this.raiderId = raiderId;
                this.totalLootDuration = totalLootDuration;
                this.currentLootDuration = currentLootDuration;
            }
        }
    }
}