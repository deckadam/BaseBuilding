using System;
using System.Threading;
using Base;
using Components.Movement;
using Cysharp.Threading.Tasks;
using Services;
using Services.Finder;
using Systems.SystemSave;
using UnityEngine;

namespace Commands
{
    public class DeckCommandMove : DeckCommand
    {
        private Vector3 _targetPosition;
        private DeckAgent _agent;

        public DeckCommandMove(Vector3 targetPosition, DeckAgent agent)
        {
            _targetPosition = targetPosition;
            _agent = agent;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var componentMovement = _agent.GetDeckComponent<DeckComponentMovement>();
            componentMovement.SetDestination(_targetPosition);

            while (componentMovement.PathPending())
            {
                await UniTask.Yield();
            }

            
            await UniTask.WaitUntil(() => componentMovement.ReachedToDestination(), cancellationToken: token);

            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new CommandMoveSaveData(_agent.UniqueId.Id, _targetPosition));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<CommandMoveSaveData>(saveData);

            _targetPosition = loadedData.targetPosition;
            _agent = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(loadedData.agentId);
        }

        [Serializable]
        private struct CommandMoveSaveData
        {
            public int agentId;
            public Vector3 targetPosition;

            public CommandMoveSaveData(int agentId, Vector3 targetPosition)
            {
                this.agentId = agentId;
                this.targetPosition = targetPosition;
            }
        }
    }
}