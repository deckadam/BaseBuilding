using System.Collections.Generic;
using Deck.Agent;
using Deck.Commands;
using Deck.Data.Item;
using Deck.Services.MapService;
using Deck.MVC;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services;
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
        private DeckAgentLoadResolver _loadResolver;
        private DeckAgentCore _coreAgentPrefab;

        [Inject]
        private void Inject(DiContainer container, DeckAgentLoadResolver loadResolver, DeckAgentCore agentCorePrefab)
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
            var playerData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(playerData);
        }

        public void GatherSaveData()
        {
            var newCoreAgentData = new DeckComponentHolderSaveDatas();
            foreach (var agent in FindObjectsOfType<DeckAgent>())
            {
                if (!agent.WillSave())
                {
                    continue;
                }

                newCoreAgentData.datas.Add(new DeckComponentHolderSaveData
                {
                    agentGuid = agent.GetUniqueId().ID,
                    componentDatas = agent.GetSaveData(),
                    prefabId = agent.PrefabId.ID
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), newCoreAgentData);
        }
    }
}