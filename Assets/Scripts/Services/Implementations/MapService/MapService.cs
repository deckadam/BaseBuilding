using Deck.Map;
using UnityEngine;
using Grid = Deck.Map.Grid;

namespace Deck.Services.Implementations.MapService
{
    public class MapService : ServiceBase
    {
        public CellResident testResident;

        public override void Initialize()
        {
            base.Initialize();
            Debug.LogError("Initializing map service");
        }

        public GamePlayMap CreateMap(string name, int width, int height)
        {
            var newMap = new GameObject()
            {
                name = name
            };
            var result = newMap.AddComponent<GamePlayMap>();
            var grid = ServiceLocator.GetService<GridService.GridService>().GenerateGrid(width, height);
            result.Initialize(grid);
            PopulateMap(result, grid, testResident);
            return result;
        }

        public void PopulateMap(GamePlayMap map, Grid grid, CellResident prefab)
        {
            var childCount = map.transform.childCount;
            if (childCount != 0)
            {
                for (var i = 0; i < map.transform.childCount; i++)
                {
                    Destroy(map.transform.GetChild(0).gameObject);
                }
            }

            for (var i = 0; i < grid.cells.GetLength(0); i++)
            {
                for (var j = 0; j < grid.cells.GetLength(1); j++)
                {
                    grid.cells[i, j].SetResident(Instantiate(prefab, grid.cells[i, j].position, Quaternion.identity, map.transform));
                }
            }
        }
    }
}