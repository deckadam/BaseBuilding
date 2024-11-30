using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;
using Deck.Save;
using Deck.Services.Finder;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deck.Commands
{
    public class DeckCommandMove : DeckCommand
    {
        private Vector3 _targetPosition;
        private DeckAgent _agent;

        public DeckCommandMove()
        {
        }

        public DeckCommandMove(Vector3 targetPosition, DeckAgent agent)
        {
            _targetPosition = targetPosition;
            _agent = agent;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var componentMovement = _agent.GetDeckComponent<DeckComponentMovement>();
            componentMovement.SetDestination(_targetPosition);
            await UniTask.NextFrame(token);
            await UniTask.WaitUntil(() => componentMovement.ReachedToDestination(), cancellationToken: token);
            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new CommandMoveSaveData(_agent.UniqueId.ID, _targetPosition));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<CommandMoveSaveData>(saveData);

            _targetPosition = loadedData.targetPosition;
            _agent = Deck.GetService<DeckServiceFinder>().GetAgent(loadedData._agentId);
        }

        [Serializable]
        private class CommandMoveSaveData
        {
            public int _agentId;

            [FormerlySerializedAs("_targetPosition")]
            public Vector3 targetPosition;

            public CommandMoveSaveData(int agentId, Vector3 targetPosition)
            {
                _agentId = agentId;
                this.targetPosition = targetPosition;
            }
        }
    }
}