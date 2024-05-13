using Deck.Agent;
using UnityEngine;

namespace Data.Component.Tree
{
    [CreateAssetMenu(fileName = "Deck Data Enemy Spawner", menuName = "Deck/Data/Component/Enemy Spawner")]
    public class DeckDataComponentEnemySpawner : DeckDataComponent
    {
        [SerializeField] private DeckAgent targetPrefab;
        [SerializeField] private int spawnDelay;
        [SerializeField] private int count;
        [SerializeField] private float wanderingSpeed;

        public DeckAgent TargetPrefab => targetPrefab;
        public float WanderingSpeed => wanderingSpeed;
        public int SpawnDelay => spawnDelay;
        public int Count => count;
    }
}