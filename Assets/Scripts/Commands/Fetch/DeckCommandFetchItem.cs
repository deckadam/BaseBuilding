using System;
using System.Threading;
using Base;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Services.Finder;
using Systems.SystemSave;
using UnityEngine;

namespace Deck.Commands.Fetch
{
    public class DeckCommandFetchItem : DeckCommand
    {
        private DeckAgent _fetcher;
        private Vector3 _fetchPosition;
        private DeckAgent _deliverTarget;

        public DeckCommandFetchItem()
        {
        }

        public DeckCommandFetchItem(DeckAgent fetcher, Vector3 fetchPosition, DeckAgent deliverTarget)
        {
            _fetcher = fetcher;
            _fetchPosition = fetchPosition;
            _deliverTarget = deliverTarget;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_fetchPosition, _fetcher).ProcessCommand(token);
            await UniTask.Delay(1000, cancellationToken: token).SuppressCancellationThrow();
            Debug.LogError("Fetch second part");
            await new DeckCommandMove(_deliverTarget.transform.position, _fetcher).ProcessCommand(token);
            Debug.LogError("Fetch completed");
            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new CommandFetchSaveData(_fetcher.UniqueId.ID, _fetchPosition, _fetcher.UniqueId.ID));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<CommandFetchSaveData>(saveData);
            var serviceFinder = global::Services.DeckServiceProvider.GetService<DeckServiceFinder>();
            
            _fetcher = serviceFinder.GetAgent(loadedData.fetcherUniqueId);
            _fetchPosition = loadedData.fetchPosition;
            _deliverTarget = serviceFinder.GetAgent(loadedData.deliverTargetUniqueId);
        }

        [Serializable]
        private class CommandFetchSaveData
        {
            public int fetcherUniqueId;
            public Vector3 fetchPosition;
            public int deliverTargetUniqueId;

            public CommandFetchSaveData(int fetcherUniqueId, Vector3 fetchPosition, int deliverTargetUniqueId)
            {
                this.fetcherUniqueId = fetcherUniqueId;
                this.fetchPosition = fetchPosition;
                this.deliverTargetUniqueId = deliverTargetUniqueId;
            }
        }
    }
}