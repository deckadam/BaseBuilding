using System;
using Base;
using Commands;
using Services;
using Services.Map;
using Services.Raid;
using Systems.SystemSave;
using Unity.Collections;
using UnityEngine;
using Utility;

namespace InGame.Agent.Raid
{
    public class DeckAgentRaider : DeckAgentHumanoid
    {
        [SerializeField, ReadOnly] private DeckEnumRaiderState currentRaiderState;

        private DeckAgentRaidController _raidController;
        private DeckServiceSession _serviceSession;
        private DeckServiceRaid _serviceRaid;

        protected override void InternalOnDeSpawned()
        {
            SetState(DeckEnumRaiderState.NotInitialized);
        }

        public void Initialize()
        {
            _serviceRaid = DeckServiceProvider.GetService<DeckServiceRaid>();
            _serviceSession = DeckServiceProvider.GetService<DeckServiceSession>();
            _raidController = _serviceRaid.GetRaidController();
            _raidController.RegisterRaider(this);
        }

        public void EnterStateInitial()
        {
            SetState(DeckEnumRaiderState.MoveTowardsChest);
            var chest = DeckServiceProvider.GetService<DeckServiceRaid>().GetMainChest();
            AddCommand(new DeckCommandMove(chest.GetPosition(), this).RegisterToOnCompleted(_raidController.OnRaiderReachedToChest));
        }

        public void EnterStateLootingChest(float startDuration = 0f)
        {
            SetState(DeckEnumRaiderState.LootingChest);
            var gameSetting = _serviceSession.GetCurrentSession().GetGameSetting();
            AddCommand(new DeckCommandLootChest(_serviceRaid.GetMainChest(), this, gameSetting.TotalLootDuration - startDuration));
            currentRaiderState = DeckEnumRaiderState.LootingChest;
        }

        public void EnterStateGuardingLooter()
        {
            SetState(DeckEnumRaiderState.GuardLooter);
            var guardingAgents = _raidController.GetGuardingAgents();
            var lootingAgent = _raidController.GetLootingRaider();
            AddCommand(new DeckCommandStayOnGuardDuringLoot(guardingAgents, lootingAgent));
        }

        public void EnterStateRunAway()
        {
            SetState(DeckEnumRaiderState.RunAway);
            var startingPoint = _serviceSession.GetCurrentSession().RaidStartPoint;
            AddCommand(new DeckCommandMove(startingPoint.GetPosition(), this).RegisterToOnCompleted(OnRunAwayFinished), true);
        }

        private void OnRunAwayFinished()
        {
            SetState(DeckEnumRaiderState.Finished);
            _raidController.OnRaiderRanAway(this);
            RequestDestroy();
        }

        public DeckEnumRaiderState GetCurrentState()
        {
            return currentRaiderState;
        }

        private void SetState(DeckEnumRaiderState state)
        {
            currentRaiderState = state;
        }

        public void ForceState(DeckEnumRaiderState state, string currentStateSaveData)
        {
            Initialize();
            switch (state)
            {
                case DeckEnumRaiderState.NotInitialized:
                    EnterStateInitial();
                    break;
                case DeckEnumRaiderState.MoveTowardsChest:
                    EnterStateInitial();
                    break;
                case DeckEnumRaiderState.LootingChest:
                    var convertedData = DeckSaveUtility.GetDeserializedData<DeckCommandLootChest.DeckCommandLootChestSaveData>(currentStateSaveData);
                    EnterStateLootingChest(convertedData.currentLootDuration);
                    break;
                case DeckEnumRaiderState.GuardLooter:
                    EnterStateGuardingLooter();
                    break;
                case DeckEnumRaiderState.RunAway:
                    EnterStateRunAway();
                    break;
                case DeckEnumRaiderState.Finished:
                    DeckLogger.Error("Finished raider should not be still active");
                    break;

                default:
                    throw new Exception($"Unknown raider state {state}");
            }
        }
    }
}