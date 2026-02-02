using System.Collections.Generic;
using System.Linq;
using Components.Health;
using Cysharp.Threading.Tasks;
using Data.Item;
using GameManager.Data.GameSetting;
using Services;
using Services.Building.Buildable;
using Services.Finder;
using Services.Map;
using Systems.SystemSave;
using Systems.SystemSave.Data;
using UnityEngine;
using Utility.Constants;
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
        private DeckGameSettingBasic[] _settings;
        private DeckGameSettingBasic _currentSetting;
        private DeckBuildable[] _allBuildables;

        [Inject]
        private void Inject(DeckLoadResolver loadResolver, DeckGameSettingBasic[] gameSettingBasic, DeckBuildable[] allBuildables)
        {
            _loadResolver = loadResolver;
            _settings = gameSettingBasic;
            _allBuildables = allBuildables;
        }

        public async UniTask CreateNewGame(DeckGameSettingBasic gameSettingBasic)
        {
            foreach (var deckBuildable in _allBuildables)
            {
                deckBuildable.ResetOverrides();
            }

            _currentSetting = gameSettingBasic;
            _currentSetting.ApplyOverrides();
            DeckSaveSystem.CreateNewSave();
            await DeckServiceProvider.GetService<DeckServiceSession>().LoadSession(gameSettingBasic);
        }

        public async UniTask<DeckGameSettingBasic> LoadGame()
        {
            foreach (var deckBuildable in _allBuildables)
            {
                deckBuildable.ResetOverrides();
            }

            var gameSettingName = DeckSaveSystem.GetData<string>(DeckConstantsSave.GameSettingKey);
            _currentSetting = _settings.First(item => gameSettingName == item.GetGameSettingName());
            _currentSetting.ApplyOverrides();
            await DeckServiceProvider.GetService<DeckServiceSession>().LoadSession(_currentSetting);
            var agentsData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(agentsData);
            return _currentSetting;
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
                    uniqueId = agent.UniqueId.Id,
                    prefabId = agent.PrefabId.Id,
                    position = agent.transform.position,
                    rotation = agent.transform.eulerAngles,
                    scale = agent.transform.localScale,
                    componentDatas = agent.GetComponentData(),
                    additionalData = agent.GetAdditionalData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), agentData);
            DeckSaveSystem.SetData(DeckConstantsSave.GameSettingKey, _currentSetting.GetGameSettingName());
        }
    }
}