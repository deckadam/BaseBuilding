using Deck.Agent;
using Deck.Agent.Chest;
using Deck.Save.Data;
using Deck.Utility.Logger;
using Zenject;

namespace Deck.Save
{
    public class DeckLoadResolver
    {
        private DeckInstanceCreator _instanceCreator;

        [Inject]
        private void Inject(DeckAgentCore agentCoreFactory, DeckAgentChest agentChestFactory, DeckInstanceCreator instanceCreator)
        {
            _instanceCreator = instanceCreator;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas agentDatas, DeckItemVisualSaveDatas itemVisualDatas)
        {
            foreach (var deckComponentHolderSaveData in agentDatas.datas)
            {
                if (deckComponentHolderSaveData.prefabId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }

                if (deckComponentHolderSaveData.uniqueId == 0)
                {
                    DeckLogger.Component($"Agent Id has not been set, skipping. {deckComponentHolderSaveData.GetType()}");
                    continue;
                }

                var agentInstance = _instanceCreator.CreateNewAgentInstance(deckComponentHolderSaveData.prefabId, deckComponentHolderSaveData.uniqueId);
                agentInstance.LoadData(deckComponentHolderSaveData);
            }

            foreach (var itemVisualData in itemVisualDatas.datas)
            {
                if (itemVisualData.prefabId == 0)
                {
                    DeckLogger.Component($"Item visual Id has not been set, skipping. {itemVisualData.GetType()}");
                    continue;
                }

                if (itemVisualData.uniqueId == 0)
                {
                    DeckLogger.Component($"Item visual Id has not been set, skipping. {itemVisualData.prefabId}  {itemVisualData.GetType()}");
                    continue;
                }

                var itemVisual = _instanceCreator.CreateNewItemVisualInstance(itemVisualData.prefabId, itemVisualData.uniqueId);
                itemVisual.LoadData(itemVisualData);
            }
        }
    }
}