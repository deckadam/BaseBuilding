using System;
using System.Collections.Generic;
using Deck.Components;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.MVC;
using Deck.Agent;
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

namespace Deck.Test
{
    public class DeckGameManager : DeckServiceBase
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeGame()
        {
            DeckMVC<DeckDataItem, IEnumerable<DeckDataItem>>.ResetController();
            DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.ResetController();
        }

        private DeckBinderGeneral _binderGeneral;
        private DiContainer _container;
        private DeckCoreAgent.Factory _agentFactory;

        [Inject]
        private void Inject(DiContainer container, DeckBinderGeneral binderGeneral, DeckCoreAgent.Factory agentFactory)
        {
            _container = container;
            _binderGeneral = binderGeneral;
            _agentFactory = agentFactory;
        }

        public override void Initialize()
        {
        }


        public void CreateNewGame()
        {
            Services.Deck.GetService<DeckGridService>().GenerateGrid(out var grid);
            Services.Deck.GetService<DeckMapService>().CreateMap();
            Services.Deck.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            Services.Deck.GetService<DeckMapService>().PopulateMap(grid);
            Services.Deck.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            Services.Deck.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            Services.Deck.GetService<DeckMapService>().SetGrid(grid);
            Services.Deck.GetService<DeckSelectionService>().SetMapData(grid.size, groundMaterial);
            Services.Deck.GetService<DeckMapService>().InitializeMap(grid, surface, ground);

            _agentFactory.Create().LoadData(Guid.NewGuid().ToString());
        }

        public void LoadGame()
        {
            Services.Deck.GetService<DeckMapService>().CreateMap();
            Services.Deck.GetService<DeckGridService>().GenerateGrid(out var grid);
            Services.Deck.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            Services.Deck.GetService<DeckMapService>().PopulateMap(grid);
            Services.Deck.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            Services.Deck.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            Services.Deck.GetService<DeckMapService>().SetGrid(grid);
            Services.Deck.GetService<DeckSelectionService>().SetMapData(grid.size, groundMaterial);
            Services.Deck.GetService<DeckMapService>().InitializeMap(grid, surface, ground);

            var playerData = DeckSaveSystem.GetData<DeckComponentHolderSaveDatas>(nameof(DeckComponentHolderSaveDatas));
            foreach (var deckCoreAgentSaveData in playerData.ids)
            {
                _agentFactory.Create().LoadData(deckCoreAgentSaveData);
            }
        }

        public void Test_FillSaveFile()
        {
            var newCoreAgentData = new DeckComponentHolderSaveDatas();
            newCoreAgentData.ids = new List<DeckComponentHolderSaveData>();
            foreach (var deckCoreAgent in Services.Deck.GetService<DeckMapService>().GetAgents())
            {
                newCoreAgentData.ids.Add(new DeckComponentHolderSaveData
                {
                    id = deckCoreAgent.GetName(),
                    componentDatas = deckCoreAgent.GetSaveData()
                });
            }

            DeckSaveSystem.SetData(nameof(DeckComponentHolderSaveDatas), newCoreAgentData);
        }
    }
}