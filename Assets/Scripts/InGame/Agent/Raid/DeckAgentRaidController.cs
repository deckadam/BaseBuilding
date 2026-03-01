using System;
using System.Collections.Generic;
using Base;
using Components;
using GameManager.Data.GameSetting;
using InGame.Agent.Chest;
using InGame.Agent.Raid;
using Services.Finder;
using Services.Map;
using Services.Raid.Events;
using Systems.SystemSave;
using UnityEngine;
using Utility;
using Utility.DataStructure;

namespace Services.Raid
{
    public class DeckAgentRaidController : DeckAgent
    {
        private DeckBuildingChest _mainChest;
        private DeckGameSettingBasic _gameSetting;
        private DeckServiceRaid _serviceRaid;
        private HashSet<DeckAgentRaider> _activeRaiders;
        private HashSet<DeckAgentRaider> _raidersReachedChest;
        private DeckAgentRaider _lootingRaider;
        private int _requiredRaiderCount;
        private bool _hasActiveRaid;

        protected override void AfterInitialize()
        {
            _activeRaiders = new HashSet<DeckAgentRaider>();
            _raidersReachedChest = new HashSet<DeckAgentRaider>();
            _serviceRaid = DeckServiceProvider.GetService<DeckServiceRaid>();
        }

        public void Initialize(DeckGameSettingBasic gameSetting)
        {
            _gameSetting = gameSetting;

            _activeRaiders.Clear();
            _raidersReachedChest.Clear();
        }

        public void GenerateRaid(DeckBuildingChest mainChest)
        {
            if (_hasActiveRaid)
            {
                DeckLogger.Error("Already has active raid going on");
                return;
            }

            _hasActiveRaid = true;
            _mainChest = mainChest;

            var raid = _gameSetting.RaidData.GetRandom();

            var startPoint = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().RaidStartPoint;
            for (var i = 0; i < raid.RaiderCount; i++)
            {
                var raiderInstance = instanceProvider.RentAgent<DeckAgentRaider>();
                raiderInstance.SetPosition(startPoint.GetPosition());
            }

            _requiredRaiderCount = raid.RaiderCount;
            DeckEventOnRaidStarted.Create().Send();
        }

        public void RegisterRaider(DeckAgentRaider raider)
        {
            _activeRaiders.Add(raider);

            if (_requiredRaiderCount == _activeRaiders.Count)
            {
                foreach (var deckAgentRaider in _activeRaiders)
                {
                    deckAgentRaider.EnterStateInitial();
                }
            }
        }

        public void OnRaiderReachedToChest(DeckAgent raider)
        {
            var raiderInstance = raider as DeckAgentRaider;
            _raidersReachedChest.Add(raiderInstance);

            var reachedCount = _raidersReachedChest.Count;
            if (reachedCount != _activeRaiders.Count)
            {
                return;
            }

            _lootingRaider = _raidersReachedChest.GetRandomElement();

            var distance = float.MaxValue;
            foreach (var temp in _raidersReachedChest)
            {
                var currentDistance = Vector3.Distance(_mainChest.GetPosition(), raider.GetPosition());
                if (!(currentDistance < distance)) continue;
                
                distance = currentDistance;
                _lootingRaider = temp;
            }

            _raidersReachedChest.Remove(_lootingRaider);
            _lootingRaider.EnterStateLootingChest();

            foreach (var guardingRaider in _raidersReachedChest)
            {
                guardingRaider.EnterStateGuardingLooter();
            }
        }

        public void OnChestLooted()
        {
            foreach (var raider in _activeRaiders)
            {
                raider.EnterStateRunAway();
            }
        }

        private void OnRaiderDeath(DeckAgentHumanoid raiderInstance)
        {
            raiderInstance.OnDeath -= OnRaiderDeath;
            var raider = raiderInstance as DeckAgentRaider;

            if (!_activeRaiders.Remove(raider))
            {
                return;
            }

            _raidersReachedChest.Remove(raider);

            if (_activeRaiders.Count != 0) return;

            _serviceRaid.OnRaidEnded(false);
            _hasActiveRaid = false;
        }

        public void OnRaiderRanAway(DeckAgentRaider raider)
        {
            _activeRaiders.Remove(raider);
            if (_activeRaiders.Count != 0) return;

            _hasActiveRaid = false;
            _serviceRaid.OnRaidEnded(true);
        }

        public override object GetAdditionalData()
        {
            var individualData = new IndividualRaiderData[_activeRaiders.Count];

            if (!_hasActiveRaid)
            {
                return new RaidControllerData(false, null);
            }

            var counter = 0;
            foreach (var raider in _activeRaiders)
            {
                var commandSaveData = DeckSaveUtility.GetSerializedData(raider.GetDeckComponent<DeckComponentCommandProcessor>().GetCurrentCommandSaveData());
                individualData[counter++] = new IndividualRaiderData(raider.GetCurrentState(), commandSaveData, raider.UniqueId.Id);
            }

            var raidControllerData = new RaidControllerData(true, individualData);
            return raidControllerData;
        }

        protected override void LoadAdditionalData(string data)
        {
            var deserializedData = DeckSaveUtility.GetDeserializedData<RaidControllerData>(data);
            var finder = DeckServiceProvider.GetService<DeckServiceFinder>();
            if (!deserializedData.HasActiveRaid)
            {
                DeckLogger.Inform("Has no active raid");
                return;
            }


            var raidService = DeckServiceProvider.GetService<DeckServiceRaid>();
            raidService.SetForceRaidStartedStatus();

            _mainChest = raidService.GetMainChest();
            _requiredRaiderCount = deserializedData.RaidersData.Length;
            _hasActiveRaid = true;

            foreach (var individualRaiderData in deserializedData.RaidersData)
            {
                var raider = finder.GetAgent<DeckAgentRaider>(individualRaiderData.UniqueId);
                raider.ForceState(individualRaiderData.State, individualRaiderData.CurrentCommandData);
            }

            DeckEventOnRaidStarted.Create().Send();
        }

        public HashSet<DeckAgentRaider> GetGuardingAgents()
        {
            return _raidersReachedChest;
        }

        public DeckAgentRaider GetLootingRaider()
        {
            return _lootingRaider;
        }

        [Serializable]
        private struct RaidControllerData
        {
            [SerializeField] private bool hasActiveRaid;
            [SerializeField] private IndividualRaiderData[] raidersData;

            public IndividualRaiderData[] RaidersData => raidersData;
            public bool HasActiveRaid => hasActiveRaid;

            public RaidControllerData(bool hasActiveRaid, IndividualRaiderData[] raidersData)
            {
                this.hasActiveRaid = hasActiveRaid;
                this.raidersData = raidersData;
            }
        }

        [Serializable]
        private struct IndividualRaiderData
        {
            [SerializeField] private DeckEnumRaiderState state;
            [SerializeField] private string currentCommandData;
            [SerializeField] private int uniqueId;

            public DeckEnumRaiderState State => state;
            public string CurrentCommandData => currentCommandData;
            public int UniqueId => uniqueId;

            public IndividualRaiderData(DeckEnumRaiderState state, string currentCommandData, int uniqueId)
            {
                this.state = state;
                this.currentCommandData = currentCommandData;
                this.uniqueId = uniqueId;
            }
        }
    }
}