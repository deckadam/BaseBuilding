using Deck;
using Deck.Save.Data;
using Zenject;

namespace Deck.Save
{
    public class DeckAgentLoadResolver
    {
        private DeckAgentCore _core;
        private DeckAgentChest _chest;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container, DeckAgentCore agentCoreFactory, DeckAgentChest agentChestFactory)
        {
            _container = container;
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
                        _container.InstantiatePrefab(_core).GetComponent<DeckAgentCore>().LoadData(deckComponentHolderSaveData);
                        break;
                    case nameof(DeckAgentChest):
                        _container.InstantiatePrefab(_chest).GetComponent<DeckAgentChest>().LoadData(deckComponentHolderSaveData);
                        break;
                }
            }
        }
    }
}