using System;
using System.Collections.Generic;
using Deck.Agent;
using Deck.Data.Buildable;
using UnityEngine;
using Utility.Enums;
using Zenject;

namespace Deck.Services.Building
{
    public class DeckBuildingService : DeckServiceBase
    {
        private DeckBuildable[] _buildables;
        private DiContainer _container;

        private Dictionary<string, DeckBuildable> _buildableDictionary;

        //1- Ground
        //2- Camera confiner
        //3- Possible collision
        private Collider[] _colliders = new Collider[3];

        [Inject]
        private void Inject(DeckBuildable[] buildables, DiContainer container)
        {
            _container = container;
            _buildables = buildables;
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

        public bool CheckIfAgentBuildableInArea(DeckBuildable buildable, Vector3 worldPosition)
        {
            if (buildable.GetAgent().GetShape() == DeckAgentShape.Box)
            {
                Debug.LogError("Box shape check");
                return CheckBoxAgent(buildable.GetAgent().GetSize(), worldPosition);
            }

            Debug.LogError("Capsule shape check");

            return CheckCapsuleAgent(buildable.GetAgent().GetSize(), worldPosition);
        }

        private bool CheckCapsuleAgent(float size, Vector3 worldPosition)
        {
            var result = Physics.OverlapSphereNonAlloc(worldPosition, size, _colliders);
            if (result > 2)
            {
                Debug.LogError(_colliders[2].gameObject.name);
            }

            Debug.LogError(result);
            return result == 2;
        }

        private bool CheckBoxAgent(float size, Vector3 worldPosition)
        {
            var result = Physics.OverlapBoxNonAlloc(worldPosition, Vector3.one * size, _colliders);
            if (result > 2)
            {
                Debug.LogError(_colliders[2].gameObject.name);
            }

            Debug.LogError(result);
            return result == 2;
        }
    }
}