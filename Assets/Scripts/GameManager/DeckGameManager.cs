using System.Collections.Generic;
using Deck.Components.Core;
using Deck.Data.Item;
using Deck.ItemVisualProviders;
using Deck.MVC;
using Deck.Save;
using Deck.Components;
using Deck.Services;
using Deck.Services.MapService;
using Services.AgentFinder;
using UnityEngine;
using Zenject;

namespace Deck.Components.Building
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
        private DeckAgentCore _coreAgentPrefab;
        private DeckItemVisualProviderBasic[] _itemVisualProviders;

        [Inject]
        private void Inject(DiContainer container, DeckLoadResolver loadResolver, DeckAgentCore agentCorePrefab, DeckItemVisualProviderBasic[] itemVisualProviders)
        {
            _container = container;
            _loadResolver = loadResolver;
            _coreAgentPrefab = agentCorePrefab;
            _itemVisualProviders = itemVisualProviders;
        }

        public async void CreateNewGame()
        {
            DeckSaveSystem.CreateNewSave();
            await Deck.GetService<DeckServiceScene>().LoadMap();
            _container.InstantiatePrefab(_coreAgentPrefab).GetComponent<DeckAgentCore>().Initialize();
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
                if (!agent.WillSave())
                {
                    continue;
                }

                agentDatas.datas.Add(new DeckComponentHolderSaveData
                {
                    uniqueId = agent.GetUniqueId().ID,
                    componentDatas = agent.GetSaveData(),
                    prefabId = agent.PrefabId.ID,
                    position = agent.transform.position,
                    rotation = agent.transform.eulerAngles,
                    scale = agent.transform.localScale
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), agentDatas);
        }
    }
}