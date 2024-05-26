using System;
using Deck.Agent;
using Deck.Agent.Chest;
using Deck.Save.Data;
using Deck.Utility.Logger;
using Zenject;

namespace Deck.Save
{
    public class DeckAgentLoadResolver
    {
        private DeckInstanceCreator _instanceCreator;

        [Inject]
        private void Inject(DeckAgentCore agentCoreFactory, DeckAgentChest agentChestFactory, DeckInstanceCreator instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas data)
        {
            foreach (var deckComponentHolderSaveData in data.datas)
            {
                if (deckComponentHolderSaveData.prefabId == 0)
                {
                    DeckLogger.Component($"Deck Agent Id has not been set, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }

                var agentInstance = _instanceCreator.CreateNewAgentInstance(deckComponentHolderSaveData.prefabId, deckComponentHolderSaveData.agentGuid);
                agentInstance.LoadData(deckComponentHolderSaveData);
            }
        }
    }
}