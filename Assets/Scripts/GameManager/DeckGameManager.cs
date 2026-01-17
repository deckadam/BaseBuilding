using System.Collections.Generic;
using Components.Health;
using Data.Item;
using GameManager.Events;
using InGame.Agent.Waiter;
using InGame.Map.Data;
using Instancing;
using Services;
using Services.Building;
using Services.Finder;
using Services.Map;
using Sirenix.OdinInspector;
using Systems.SystemSave;
using Systems.SystemSave.Data;
using UnityEngine;
using Utility;
using Utility.MVC;
using Zenject;

namespace GameManager
{
    public class DeckGameManager : DeckServiceBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckDataItem, IEnumerable<DeckDataItem>>.ResetController();
            DeckMVC<DeckComponentHealth, IEnumerable<DeckComponentHealth>>.ResetController();
        }

        private DeckLoadResolver _loadResolver;

        [Inject]
        private void Inject(DeckLoadResolver loadResolver)
        {
            _loadResolver = loadResolver;
        }

        public async void CreateNewGame(DeckDataMap mapData)
        {
            DeckSaveSystem.CreateNewSave();
            await DeckServiceProvider.GetService<DeckServiceScene>().LoadMap();
            DeckEventOnNewMapCreated.Create(mapData).Send();
        }

        public async void LoadGame()
        {
            await DeckServiceProvider.GetService<DeckServiceScene>().LoadMap();
            var agentsData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(agentsData);
        }

        public void GatherSaveData()
        {
            DeckServiceProvider.BeforeSaveRequest();

            var agents = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgents();
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