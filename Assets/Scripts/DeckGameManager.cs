using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Components;
using Deck.Data.General;
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
            Deck.GetService<DeckGridService>().GenerateGrid(out var grid);
            Deck.GetService<DeckMapService>().CreateMap();
            Deck.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            Deck.GetService<DeckMapService>().PopulateMap(grid);
            Deck.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            Deck.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            Deck.GetService<DeckMapService>().SetGrid(grid);
            Deck.GetService<DeckSelectionService>().SetMapData(grid.size, groundMaterial);
            Deck.GetService<DeckMapService>().InitializeMap(grid, surface, ground);

            _agentFactory.Create().LoadData(Guid.NewGuid().ToString());
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
            foreach (var deckCoreAgentSaveData in playerData.ids)
            {
                _agentFactory.Create().LoadData(deckCoreAgentSaveData);
            }
        }

        public void Test_FillSaveFile()
        {
            var newCoreAgentData = new DeckComponentHolderSaveDatas();
            newCoreAgentData.ids = new List<DeckComponentHolderSaveData>();
            foreach (var deckCoreAgent in Deck.GetService<DeckMapService>().GetAgents())
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