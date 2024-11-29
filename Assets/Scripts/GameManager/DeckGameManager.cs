using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Item;
using Deck.InGame.Agent.Waiter;
using Deck.ItemVisualProviders;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Services.Map;
using Deck.Utility.MVC;
using UnityEngine;
using Zenject;

namespace Deck.GameManager
{
    public class DeckGameManager : DeckServiceBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckDataItem, IEnumerable<DeckDataItem>>.ResetController();
            DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.ResetController();
        }

        private DiContainer _container;
        private DeckLoadResolver _loadResolver;
        private DeckAgentWaiter _waiterAgentPrefab;
        private DeckItemVisualProviderBasic[] _itemVisualProviders;

        [Inject]
        private void Inject(DiContainer container, DeckLoadResolver loadResolver, DeckAgentWaiter agentWaiterPrefab, DeckItemVisualProviderBasic[] itemVisualProviders)
        {
            _container = container;
            _loadResolver = loadResolver;
            _waiterAgentPrefab = agentWaiterPrefab;
            _itemVisualProviders = itemVisualProviders;
        }

        public async void CreateNewGame()
        {
            DeckSaveSystem.CreateNewSave();
            await Deck.GetService<DeckServiceScene>().LoadMap();
            _container.InstantiatePrefab(_waiterAgentPrefab).GetComponent<DeckAgentWaiter>().Initialize();
        }

        public async void LoadGame()
        {
            await Deck.GetService<DeckServiceScene>().LoadMap();
            var agentsData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(agentsData);
        }

        public void GatherSaveData()
        {
            Deck.BeforeSaveRequest();

            var agents = Deck.GetService<DeckServiceFinder>().GetAgents();
            var agentDatas = new DeckComponentHolderSaveDatas();
            foreach (var agent in agents)
            {
                if (!agent.WillSave)
                {
                    continue;
                }

                agentDatas.datas.Add(new DeckComponentHolderSaveData
                {
                    uniqueId = agent.UniqueId.ID,
                    componentDatas = agent.GetSaveData(),
                    prefabId = agent.PrefabId.ID,
                    position = agent.transform.position,
                    rotation = agent.transform.eulerAngles,
                    scale = agent.transform.localScale,
                    additionalData = agent.GetAdditionalData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), agentDatas);
        }
    }
}