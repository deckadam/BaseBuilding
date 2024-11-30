using System.Collections.Generic;
using Deck.Components;
using Deck.Data.Item;
using Deck.InGame.Agent.Waiter;
using Deck.Instancing;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Services.Map;
using Deck.Utility.MVC;
using Deck.Waiter;
using Sirenix.OdinInspector;
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

        private DeckLoadResolver _loadResolver;
        private DeckAgentWaiter _waiterAgentPrefab;
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckLoadResolver loadResolver, DeckAgentWaiter agentWaiterPrefab, DeckInstanceProvider instanceProvider)
        {
            _loadResolver = loadResolver;
            _waiterAgentPrefab = agentWaiterPrefab;
            _instanceProvider = instanceProvider;
        }

        public async void CreateNewGame()
        {
            DeckSaveSystem.CreateNewSave();
            await Deck.GetService<DeckServiceScene>().LoadMap();
            var newWaiter = _instanceProvider.RentAgent(_waiterAgentPrefab.PrefabId);
            newWaiter.transform.parent = DeckServiceScene.GetMap().transform;
            newWaiter.Initialize();
        }

        [Button]
        private void CreateWaiter()
        {
            var newWaiter = _instanceProvider.RentAgent(_waiterAgentPrefab.PrefabId);
            newWaiter.transform.parent = DeckServiceScene.GetMap().transform;
            newWaiter.Initialize();
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