using Cysharp.Threading.Tasks;
using Data.Component.Tree;
using Deck.Commands;
using Deck.Services.MapService;
using Deck.Utility.Logger;
using UnityEngine;
using Zenject;

namespace Deck.Agent.EnemySpawner
{
    public class DeckAgentEnemySpawner : MonoBehaviour
    {
        [SerializeField] private DeckDataComponentEnemySpawner spawnData;

        private int _activeEnemyCount;
        private DiContainer _container;

        [Inject]
        private void Inject(DiContainer container)
        {
            _container = container;
        }

        private async void Awake()
        {
            var token = gameObject.GetCancellationTokenOnDestroy();
            while (!token.IsCancellationRequested)
            {
                var isCanceled = await UniTask.Delay(spawnData.SpawnDelay, cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled)
                {
                    return;
                }

                SpawnEnemy();
            }
        }

        private void SpawnEnemy()
        {
            if (_activeEnemyCount >= spawnData.Count)
            {
                return;
            }

            DeckLogger.Inform($"Spawned enemy {spawnData.TargetPrefab.name}");

            var newEnemy = _container.InstantiatePrefab(spawnData.TargetPrefab, transform.position, transform.rotation, DeckServiceScene.GetMap().transform);
            newEnemy.GetComponent<DeckAgent>().OnAgentDeath += RemoveEnemy;

            _activeEnemyCount++;
        }

        private void RemoveEnemy(DeckAgent deadAgent)
        {
            deadAgent.GetComponent<DeckAgent>().OnAgentDeath -= RemoveEnemy;
            _activeEnemyCount--;
        }
    }
}