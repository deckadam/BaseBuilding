using Deck.Agent;
using Deck.Map.Agent.Chest;
using Deck.Save.Data;
using Zenject;

namespace Deck.Save
{
    public class DeckAgentLoadResolver
    {
        private DeckAgentCore.Factory _core;
        private DeckAgentChest.Factory _chest;

        [Inject]
        private void Inject(DeckAgentCore.Factory agentCoreFactory, DeckAgentChest.Factory agentChestFactory)
        {
            _core = agentCoreFactory;
            _chest = agentChestFactory;
        }

        public void ResolveAndLoad(DeckComponentHolderSaveDatas data)
        {
            foreach (var deckComponentHolderSaveData in data.datas)
            {
                switch (deckComponentHolderSaveData.id)
                {
                    case nameof(DeckAgentCore):
                        _core.Create().LoadData(deckComponentHolderSaveData);
                        break;
                    case nameof(DeckAgentChest):
                        _chest.Create().LoadData(deckComponentHolderSaveData);
                        break;
                }
            }
        }
    }
}