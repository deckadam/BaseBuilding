using Data.Buildable;
using UnityEngine;

namespace InGame.Map.Data
{
    [CreateAssetMenu(menuName = "Deck/Data/General/Map", fileName = "Deck Data Map")]
    public class DeckDataMap : ScriptableObject
    {
        [SerializeField] private Vector2Int mapSize;
        [SerializeField] private DeckBuildable wallBuildable;

        public Vector2Int MapSize => mapSize;
        public DeckBuildable WallBuildable => wallBuildable;
    }
}