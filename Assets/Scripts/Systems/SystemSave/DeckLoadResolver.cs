using System.Collections.Generic;
using System.Linq;
using Instancing;
using Systems.SystemSave.Data;
using Utility;
using Zenject;

namespace Systems.SystemSave
{
    public class DeckLoadResolver
    {
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas allAgentData)
        {
            var resolvedData = new Dictionary<int, List<DeckComponentHolderSaveData>>();
            foreach (var data in allAgentData.data)
            {
                if (data.prefabId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {data.GetType()}");
                    continue;
                }

                if (data.uniqueId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {data.GetType()}");
                    continue;
                }

                if (resolvedData.TryGetValue(data.prefabId, out var existingList))
                {
                    existingList.Add(data);
                }
                else
                {
                    var newList = new List<DeckComponentHolderSaveData>();
                    resolvedData[data.prefabId] = newList;
                    newList.Add(data);
                }
            }

            foreach (var bulkData in resolvedData)
            {
                var ids = bulkData.Value.Select(item => item.uniqueId).ToArray();
                var agentInstances = _instanceProvider.BulkRentAgent(bulkData.Key, ids);

                for (var index = 0; index < agentInstances.Length; index++)
                {
                    var agentInstance = agentInstances[index];
                    agentInstance.LoadData(bulkData.Value[index]);
                }

                foreach (var agentInstance in agentInstances)
                {
                    agentInstance.AfterLoadingFinished();
                }
            }
        }
    }
}