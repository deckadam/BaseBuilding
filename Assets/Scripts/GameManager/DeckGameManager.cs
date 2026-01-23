using System.Collections.Generic;
using Components.Health;
using Cysharp.Threading.Tasks;
using Data.Item;
using GameManager.Data;
using GameManager.Data.GameSetting;
using GameManager.Events;
using InGame.Agent.Waiter;
using Instancing;
using Services;
using Services.Building;
using Services.Finder;
using Services.Map;
using Systems.SystemSave;
using Systems.SystemSave.Data;
using UnityEngine;
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
        private DeckGameSettingBasic _gameSettingBasic;
        
        [Inject]
        private void Inject(DeckLoadResolver loadResolver,DeckGameSettingBasic[] gameSettingBasic)
        {
            _loadResolver = loadResolver;
            _gameSettingBasic = gameSettingBasic[0];
        }

        public async UniTask CreateNewGame(DeckGameSettingBasic gameSettingBasic)
        {
            DeckSaveSystem.CreateNewSave();
            await DeckServiceProvider.GetService<DeckServiceSession>().LoadSession(gameSettingBasic);
        }

        public async void LoadGame()
        {
            await DeckServiceProvider.GetService<DeckServiceSession>().LoadSession(_gameSettingBasic);
            var agentsData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(agentsData);
        }

        public void GatherSaveData()
        {
            DeckServiceProvider.BeforeSaveRequest();

            var agents = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgents();
            var agentData = new DeckComponentHolderSaveDatas();
            foreach (var agent in agents)
            {
                if (!agent.WillSave)
                {
                    continue;
                }

                agentData.data.Add(new DeckComponentHolderSaveData
                {
                    uniqueId = agent.UniqueId.ID,
                    prefabId = agent.PrefabId.ID,
                    position = agent.transform.position,
                    rotation = agent.transform.eulerAngles,
                    scale = agent.transform.localScale,
                    componentDatas = agent.GetComponentData(),
                    additionalData = agent.GetAdditionalData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), agentData);
        }
    }
}