using CameraController;
using GameManager.Data.GameSetting;
using Services.Raid.Tools;
using Unity.AI.Navigation;
using UnityEngine;

namespace InGame.Map
{
    public class DeckSession : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private DeckRaidStartPoint raidStartPoint;
        [SerializeField] private BoxCollider mapBoundCollider;
        [SerializeField] private Transform ground;

        public DeckRaidStartPoint RaidStartPoint => raidStartPoint;

        private DeckBaseCameraController[] _cameraControllers;
        private DeckGameSettingBasic _currentGameSetting;

        private Transform _selfTransform;

        public void Initialize(DeckGameSettingBasic gameSetting)
        {
            _selfTransform = transform;
            _currentGameSetting = gameSetting;
            var mapSize = gameSetting.GetMapSize();

            var max = Mathf.Max(mapSize.x, mapSize.y);

            ground.transform.localPosition = new Vector3(mapSize.x / 2f, 0, mapSize.y / 2f);
            ground.transform.localScale = new Vector3(max, 0, max) / 5f;
            ground.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(max * 2f, max * 2f);

            mapBoundCollider.center = new Vector3(mapSize.x / 2f, 0, mapSize.y / 2f);
            mapBoundCollider.size = new Vector3(mapSize.x + 25f, 100, mapSize.y + 25f);

            navMeshSurface.transform.position = ground.position;
            navMeshSurface.size = mapBoundCollider.size;
            if (!gameSetting.IsTestRun())
            {
                navMeshSurface.BuildNavMesh();
            }

            _cameraControllers = FindObjectsByType<DeckBaseCameraController>(FindObjectsSortMode.None);
            foreach (var cameraController in _cameraControllers)
            {
                cameraController.Initialize();
            }
        }

        public Collider GetCameraCollider()
        {
            return mapBoundCollider;
        }

        public DeckGameSettingBasic GetGameSetting()
        {
            return _currentGameSetting;
        }

        public Transform GetSelfTransform()
        {
            return _selfTransform;
        }
    }
}