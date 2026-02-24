using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Agent.Raid;
using Services;
using Services.Finder;
using Systems.SystemSave;

namespace Commands
{
    public class DeckCommandStayOnGuardDuringLoot : DeckCommand
    {
        private HashSet<DeckAgentRaider> _onGuardAgents;
        private bool _isStillGuarding;

        public DeckCommandStayOnGuardDuringLoot(HashSet<DeckAgentRaider> onGuardAgents, DeckAgentRaider agentToGuard)
        {
            _onGuardAgents = onGuardAgents;
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
            return DeckSaveUtility.GetSerializedData(new CommandLootChestSaveData(_onGuardAgents.Select(item => item.UniqueId.Id).ToArray()));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<CommandLootChestSaveData>(saveData);
            _onGuardAgents = new HashSet<DeckAgentRaider>();

            foreach (var agentId in loadedData.agentIds)
            {
                _onGuardAgents.Add(DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(agentId) as DeckAgentRaider);
            }
        }

        [Serializable]
        private struct CommandLootChestSaveData
        {
            public int[] agentIds;

            public CommandLootChestSaveData(int[] agentIds)
            {
                this.agentIds = agentIds;
            }
        }
    }
}