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
        private DeckResolverData _resolverData;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container, DeckAgentCore agentCoreFactory, DeckAgentChest agentChestFactory, DeckResolverData resolverData)
        {
            _container = container;
            _resolverData = resolverData;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas data)
        {
            foreach (var deckComponentHolderSaveData in data.datas)
            {
                if (deckComponentHolderSaveData.prefabId == string.Empty)
                {
                    DeckLogger.Component($"Deck Agent Id is 0, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }

                var agentPrefab = _resolverData.GetAgentById(Guid.Parse(deckComponentHolderSaveData.prefabId));
                var agentInstance = _container.InstantiatePrefab(agentPrefab).GetComponent<DeckAgent>();
                agentInstance.Initialize(deckComponentHolderSaveData.agentGuid);
                agentInstance.LoadData(deckComponentHolderSaveData);
            }
        }
    }
}