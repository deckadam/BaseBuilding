using System.Collections.Generic;
using System.Linq;
using Deck.Agent;
using Deck.Commands;
using Deck.Data.Item;
using Deck.Services.MapService;
using Deck.MVC;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services;
using Services.AgentFinder;
using UnityEngine;
using Zenject;

namespace Deck.UI
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

        [Inject]
        private void Inject(DiContainer container, DeckLoadResolver loadResolver, DeckAgentCore agentCorePrefab)
        {
            _container = container;
            _loadResolver = loadResolver;
            _coreAgentPrefab = agentCorePrefab;
        }

        public async void CreateNewGame()
        {
            await Deck.GetService<DeckServiceScene>().LoadMap(false);
            _container.InstantiatePrefab(_coreAgentPrefab).GetComponent<DeckAgentCore>().Initialize();
        }

        public async void LoadGame()
        {
            await Deck.GetService<DeckServiceScene>().LoadMap(true);
            var agentDatas = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            var itemVisualDatas = DeckSaveSystem.GetData<DeckItemVisualSaveDatas>(nameof(DeckItemVisualSaveDatas));
            _loadResolver.ResolveAndLoad(agentDatas, itemVisualDatas);
        }

        public void GatherSaveData()
        {
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
                    prefabId = agent.PrefabId.ID
                });
            }

            var itemVisuals = Deck.GetService<DeckServiceFinder>().GetItemVisuals();
            var itemVisualDatas = new DeckItemVisualSaveDatas();
            foreach (var itemVisual in itemVisuals)
            {
                itemVisualDatas.datas.Add(new DeckItemVisualSaveData()
                {
                    prefabId = itemVisual.PrefabId.ID,
                    uniqueId = itemVisual.UniqueId.ID,
                    position = itemVisual.transform.position,
                    rotation = itemVisual.transform.rotation.eulerAngles,
                    isOnTheGround = itemVisual.IsOnTheGround,
                    additionalData = itemVisual.GetAdditionalData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), agentDatas);
            DeckSaveSystem.SetData(nameof(DeckItemVisualSaveDatas), itemVisualDatas);
        }
    }
}