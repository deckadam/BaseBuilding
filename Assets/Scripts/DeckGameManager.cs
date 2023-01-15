using System.Collections.Generic;
using Deck.Components;
using Deck.Data.General;
using Deck.Data.Item;
using Deck.MVC;
using Deck.Player;
using Deck.Services;
using Deck.Services.Implementations.CameraService;
using Deck.Services.Implementations.CellSelectionService;
using Deck.Services.Implementations.GridService;
using Deck.Services.Implementations.Level;
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
            DeckMVC<DeckItem, IEnumerable<DeckItem>>.ResetController();
            DeckMVC<DeckHealthComponent, IEnumerable<DeckHealthComponent>>.ResetController();
        }

        private DeckGeneralData _generalData;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container, DeckGeneralData generalData)
        {
            _container = container;
            _generalData = generalData;
        }

        public void CreateNewGame()
        {
            DeckServiceLocator.GetService<DeckMapService>().CreateMap();
            DeckServiceLocator.GetService<DeckGridService>().GenerateGrid(out var grid);
            DeckServiceLocator.GetService<DeckMapService>().CreateGround(grid, out var ground, out var groundMaterial);
            DeckServiceLocator.GetService<DeckMapService>().PopulateMap(grid);
            DeckServiceLocator.GetService<DeckNavigationService>().GenerateNavigation(out var surface);
            DeckServiceLocator.GetService<DeckCameraService>().GenerateCameraBounds(grid);
            DeckServiceLocator.GetService<DeckLevelService>().SetGrid(grid);
            DeckServiceLocator.GetService<DeckCellSelectionService>().SetMapData(grid.size, groundMaterial);
            DeckServiceLocator.GetService<DeckMapService>().InitializeMap(grid, surface, ground);


            var temp = _container.InstantiatePrefab(_generalData.coreAgentPrefab, Vector3.zero, Quaternion.identity, DeckMapService.map.transform);
            var tempAgent = temp.GetComponent<DeckCoreAgent>();
            DeckServiceLocator.GetService<DeckLevelService>().AddCoreAgent(tempAgent);
        }
    }
}