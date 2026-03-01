using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Agent.Raid;
using Services;
using Services.Finder;
using Systems.SystemSave;
using UnityEngine;

namespace Commands
{
    public class DeckCommandStayOnGuardDuringLoot : DeckCommand
    {
        private HashSet<DeckAgentRaider> _onGuardAgents;
        private DeckAgentRaider _agentToGuard;
        private bool _isStillGuarding;

        public DeckCommandStayOnGuardDuringLoot(HashSet<DeckAgentRaider> onGuardAgents, DeckAgentRaider agentToGuard)
        {
            _onGuardAgents = onGuardAgents;
            _agentToGuard = agentToGuard;
            _isStillGuarding = true;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            while (_isStillGuarding)
            {
                await UniTask.NextFrame(token);
            }

            return await base.ProcessCommand(token);
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new DeckCommandStayOnGuardDuringLootSaveData(_onGuardAgents.Select(item => item.UniqueId.Id).ToArray()));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<DeckCommandStayOnGuardDuringLootSaveData>(saveData);
            _onGuardAgents = new HashSet<DeckAgentRaider>();

            foreach (var agentId in loadedData.agentIds)
            {
                _onGuardAgents.Add(DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(agentId) as DeckAgentRaider);
            }
        }

        [Serializable]
        private struct DeckCommandStayOnGuardDuringLootSaveData
        {
            public int[] agentIds;

            public DeckCommandStayOnGuardDuringLootSaveData(int[] agentIds)
            {
                this.agentIds = agentIds;
            }
        }
    }
}