using System.Linq;
using Services;
using Services.Building;
using UnityEngine;

namespace GameManager.Data.GameSetting
{
    [CreateAssetMenu(menuName = "Deck/Data/General/Game Settings", fileName = "Deck Game Setting Basic")]
    public class DeckGameSettingBasic : ScriptableObject
    {
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private DeckWallType wallType;
        [SerializeField] private DeckWallData[] allWallData;

        public DeckWallType GetWallType()
        {
            return wallType;
        }

        public void OnSettingLoadedNewGame()
        {
            var wallData = allWallData.First(item => item.WallType == wallType);
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInRectBulk(wallData.WallBuildable, Vector2Int.zero, mapSize);
        }
    }
}