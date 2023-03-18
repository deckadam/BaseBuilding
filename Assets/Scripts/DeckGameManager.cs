using System.Collections.Generic;
using Deck.Agent;
using Deck.Components;
using Deck.Data.Item;
using Deck.MVC;
using Deck.Save;
using Deck.Save.Data;
using Deck.Services;
using Deck.Services.Implementations.CameraService;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.MapService;
using Deck.Services.Implementations.Navigation;
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
        private DeckAgentLoadResolver _loadResolver;
        private DeckAgentCore.Factory _agentFactory;

        [Inject]
        private void Inject(DiContainer container, DeckAgentLoadResolver loadResolver, DeckAgentCore.Factory agentFactory)
        {
            _container = container;
            _loadResolver = loadResolver;
            _agentFactory = agentFactory;
        }

        public void CreateNewGame()
        {
            Deck.GetService<DeckGridService>().GenerateGrid(out var grid);
            Deck.GetService<DeckMapService>().CreateMap();
            Deck.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            Deck.GetService<DeckMapService>().PopulateMap(grid);
            Deck.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            Deck.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            Deck.GetService<DeckMapService>().SetGrid(grid);
            Deck.GetService<DeckSelectionService>().SetMapData(grid.size, groundMaterial);
            Deck.GetService<DeckMapService>().InitializeMap(grid, surface, ground);
            _agentFactory.Create().StartWithClearData();
        }

        public void LoadGame()
        {
            Deck.GetService<DeckMapService>().CreateMap();
            Deck.GetService<DeckGridService>().GenerateGrid(out var grid);
            Deck.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            Deck.GetService<DeckMapService>().PopulateMap(grid);
            Deck.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            Deck.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            Deck.GetService<DeckMapService>().SetGrid(grid);
            Deck.GetService<DeckSelectionService>().SetMapData(grid.size, groundMaterial);
            Deck.GetService<DeckMapService>().InitializeMap(grid, surface, ground);

            var playerData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            _loadResolver.ResolveAndLoad(playerData);
        }

        public void Test_FillSaveFile()
        {
            var newCoreAgentData = new DeckComponentHolderSaveDatas();
            foreach (var deckCoreAgent in FindObjectsOfType<DeckAgent>())
            {
                if (!deckCoreAgent.WillSave())
                {
                    continue;
                }

                newCoreAgentData.datas.Add(new DeckComponentHolderSaveData
                {
                    id = deckCoreAgent.GetId(),
                    componentDatas = deckCoreAgent.GetSaveData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), newCoreAgentData);
        }
    }
}