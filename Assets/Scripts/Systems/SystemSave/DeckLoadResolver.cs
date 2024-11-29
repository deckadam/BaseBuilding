using System.Collections.Generic;
using Deck.Base;
using Deck.Instancing;
using Deck.Utility;
using Zenject;

namespace Deck.Save
{
    public class DeckLoadResolver
    {
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas agentDatas)
        {
            var resolvedData = new List<(DeckAgent, DeckComponentHolderSaveData)>();
            foreach (var data in agentDatas.datas)
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

                var agentInstance = _instanceProvider.RentAgent(data.prefabId, data.uniqueId);
                resolvedData.Add((agentInstance, data));
            }

            foreach (var tuple in resolvedData)
            {
                tuple.Item1.LoadData(tuple.Item2);
            }

            foreach (var tuple in resolvedData)
            {
                tuple.Item1.AfterLoadingFinished();
            }
        }
    }
}