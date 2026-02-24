using System.Collections.Generic;
using Base;
using GameManager.Data.GameSetting;
using InGame.Agent.Chest;
using InGame.Agent.Raid;
using Instancing;
using Services.Map;
using UnityEngine;
using Utility;
using Utility.DataStructure;
using Zenject;

namespace Services.Raid
{
    public class DeckAgentRaidController : DeckAgent
    {
        private DeckBuildingChest _mainChest;
        private DeckInstanceProvider _instanceProvider;
        private DeckGameSettingBasic _gameSetting;
        private DeckServiceRaid _serviceRaid;
        private HashSet<DeckAgentRaider> _activeRaiders;
        private HashSet<DeckAgentRaider> _raidersReachedChest;

        private DeckAgentRaider _lootingRaider;
        private int _requiredRaiderCount;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public override void OnSpawned()
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
            _mainChest = mainChest;
            var raid = _gameSetting.RaidData.GetRandom();

            var startPoint = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().RaidStartPoint;
            for (var i = 0; i < raid.RaiderCount; i++)
            {
                var raiderInstance = _instanceProvider.RentAgent<DeckAgentRaider>();
                raiderInstance.SetPosition(startPoint.GetPosition());
            }

            _requiredRaiderCount = raid.RaiderCount;
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
            if (raiderInstance == null)
            {
                DeckLogger.Error("Miscast to raider", raider.gameObject);
                return;
            }

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
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    _lootingRaider = temp;
                }
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

            if (_activeRaiders.Count == 0)
            {
                _serviceRaid.OnRaidEnded(false);
            }
        }

        public void OnRaiderRanAway(DeckAgentRaider raider)
        {
            _activeRaiders.Remove(raider);

            if (_activeRaiders.Count == 0)
            {
                _serviceRaid.OnRaidEnded(true);
            }
        }
    }
}