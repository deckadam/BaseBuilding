using Cysharp.Threading.Tasks;
using EventManager;
using GameManager.Events;
using InGame.Agent.Chest;
using InGame.Agent.Chest.Events;
using Services.Building.Events;
using Services.PathFinding.Events;
using UnityEngine;
using UnityEngine.AI;

namespace Services.PathFinding
{
    public class DeckServicePathFinding : DeckServiceBase
    {
        private NavMeshAgent _pathFindingAgent;
        private DeckBuildingChest _mainChest;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventOnGameSettingsLoaded>(OnSessionStarted);
            DeckEventManager.Register<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Register<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Register<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);
        }

        protected override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnGameSettingsLoaded>(OnSessionStarted);
            DeckEventManager.Unregister<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Unregister<DeckEventOnAnythingBuilt>(OnAnythingBuilt);
            DeckEventManager.Unregister<DeckEventOnAnythingDestroyed>(OnAnythingDestroyed);

            _mainChest = null;
        }

        private void OnMainChestBuilt(DeckEventOnMainChestBuilt obj)
        {
            _mainChest = obj.chest;
        }

        private void OnAnythingDestroyed(DeckEventOnAnythingDestroyed obj)
        {
            CheckPathStatus();
        }

        private void OnAnythingBuilt(DeckEventOnAnythingBuilt obj)
        {
            CheckPathStatus();
        }

        private async void OnSessionStarted(DeckEventOnGameSettingsLoaded obj)
        {
            var newGO = new GameObject
            {
                transform =
                {
                    parent = transform
                },
                name = "PathFinding"
            };
            var mapSize = obj.gameSetting.GetMapSize();
            var pos = new Vector3(mapSize.x / 2f, 0, -10f);

            newGO.transform.position = pos;

            await UniTask.NextFrame();
            await UniTask.NextFrame();
            await UniTask.NextFrame();

            _pathFindingAgent = newGO.AddComponent<NavMeshAgent>();

            CheckPathStatus();
        }


        private async void CheckPathStatus()
        {
            if (_mainChest == null)
            {
                return;
            }

            _pathFindingAgent.SetDestination(_mainChest.GetPosition());

            await UniTask.WaitWhile(() => _pathFindingAgent.pathPending);

            DeckEventOnPathStatusChanged.Create(_pathFindingAgent.path.status == NavMeshPathStatus.PathComplete);
        }
    }
}