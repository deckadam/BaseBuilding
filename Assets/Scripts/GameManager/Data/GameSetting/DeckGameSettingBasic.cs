using System.Linq;
using GameManager.Data.GameSetting.Override;
using Services;
using Services.Building;
using Services.Raid.Data;
using UnityEngine;

namespace GameManager.Data.GameSetting
{
    [CreateAssetMenu(menuName = "Deck/Data/General/Game Settings", fileName = "Deck Game Setting Basic")]
    public class DeckGameSettingBasic : ScriptableObject
    {
        [SerializeField] private bool isTestRun;
        [SerializeField] private string gameSettingName;
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private DeckWallType wallType;
        [SerializeField] private DeckWallData[] allWallData;
        [SerializeField] private DeckDataRaid[] raidData;
        [SerializeField] private DeckGameSettingBuildableOverride[] buildableOverrides;
        [SerializeField] private float totalLootDuration;
        
        public DeckWallType WallType => wallType;
        public DeckDataRaid[] RaidData => raidData;
        public float TotalLootDuration => totalLootDuration;

        public void OnSettingLoadedNewGame()
        {
            var wallData = allWallData.First(item => item.WallType == wallType);
            DeckServiceProvider.GetService<DeckServiceBuilding>().BuildInRectBulk(wallData.WallBuildable, Vector2Int.zero, mapSize);
        }

        public Vector2Int GetMapSize()
        {
            return mapSize;
        }

        public string GetGameSettingName()
        {
            return gameSettingName;
        }

        public void ApplyOverrides()
        {
            foreach (var deckGameSettingBuildableOverride in buildableOverrides)
            {
                deckGameSettingBuildableOverride.Buildable.ApplyOverride(deckGameSettingBuildableOverride);
            }
        }

        public bool IsTestRun()
        {
            return isTestRun;
        }
    }
}