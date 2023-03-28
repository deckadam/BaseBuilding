using System;
using System.Collections.Generic;
using Deck;
using Deck.Data.Buildable;
using Deck.Data.General;
using Unity.VisualScripting;
using UnityEngine;
using Utility.Enums;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckServiceBuilding : DeckServiceBase
    {
        private DeckBuildable[] _buildables;
        private DiContainer _container;
        private DeckDataBuilding _buildingData;
        private Dictionary<string, DeckBuildable> _buildableDictionary;

        private GameObject _silouetteMaster;
        private List<MeshRenderer> _renderer = new();
        private List<MeshFilter> _filter = new();
        private DeckBuildable _activeBuildable;

        //1- Ground
        //2- Camera confiner
        //3- Possible collision
        private Collider[] _colliders = new Collider[3];

        [Inject]
        private void Inject(DeckBuildable[] buildables, DeckDataBuilding buildingData, DiContainer container)
        {
            _buildables = buildables;
            _buildingData = buildingData;
            _container = container;
        }

        private void Awake()
        {
            _silouetteMaster = new GameObject();
            _buildableDictionary = new Dictionary<string, DeckBuildable>();
            foreach (var deckBuildable in _buildables)
            {
                _buildableDictionary[deckBuildable.GetName()] = deckBuildable;
            }
        }

        public T Build<T>(DeckBuildable buildable) where T : DeckAgent
        {
            return _container.InstantiatePrefab(buildable.GetAgent().gameObject).GetComponent<T>();
        }

        public DeckBuildable GetBuildable(string name)
        {
            if (_buildableDictionary.TryGetValue(name, out var match))
            {
                return match;
            }

            throw new Exception("Buildable not found");
        }


        public void StartSilouette(DeckBuildable buildable)
        {
            _activeBuildable = buildable;
            var silouetteData = _activeBuildable.GetSilouette();
            for (var i = 0; i < silouetteData.Length; i++)
            {
                var data = silouetteData[i];
                var newObject = new GameObject();
                var newFilter = newObject.AddComponent<MeshFilter>();
                newFilter.mesh = data.GetMesh();
                var newRenderer = newObject.AddComponent<MeshRenderer>();
                newRenderer.material = _buildingData.GetAvailableMaterial();

                newObject.transform.SetParent(_silouetteMaster.transform);
                newObject.transform.localPosition = data.GetPosition();
                newObject.transform.eulerAngles = data.GetRotation();

                _filter.Add(newFilter);
                _renderer.Add(newRenderer);
            }
        }

        public void UpdateSilouette(Vector3 worldPosition)
        {
            var isPlacable = CheckIfAgentBuildableInArea(_activeBuildable, worldPosition);
            var material = isPlacable ? _buildingData.GetAvailableMaterial() : _buildingData.GetUnavailableMaterial();
            foreach (var meshRenderer in _renderer)
            {
                meshRenderer.material = material;
            }

            _silouetteMaster.transform.position = worldPosition;
        }

        public void StopSilouette()
        {
            foreach (var meshFilter in _filter)
            {
                Destroy(meshFilter.gameObject);
            }

            _filter.Clear();
            _renderer.Clear();
        }

        public bool CheckIfAgentBuildableInArea(DeckBuildable buildable, Vector3 worldPosition)
        {
            if (buildable.GetAgent().GetShape() == DeckAgentShape.Box)
            {
                return CheckBoxAgent(buildable.GetAgent().GetSize(), worldPosition);
            }
            else
            {
                return CheckCapsuleAgent(buildable.GetAgent().GetSize(), worldPosition);
            }
        }

        private bool CheckCapsuleAgent(float size, Vector3 worldPosition)
        {
            var result = Physics.OverlapSphereNonAlloc(worldPosition, size, _colliders);
            return result == 2;
        }

        private bool CheckBoxAgent(float size, Vector3 worldPosition)
        {
            var result = Physics.OverlapBoxNonAlloc(worldPosition, Vector3.one * size, _colliders);
            return result == 2;
        }
    }
}