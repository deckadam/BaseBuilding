using System;
using System.Text;
using Deck.Component;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Components
{
    public class DeckMovementComponent : IDeckComponent
    {
        private DeckComponentHolder _componentHolder;
        private NavMeshAgent _navMeshAgent;

        public void Initialize(DeckComponentHolder agent)
        {
            _componentHolder = agent;
            _navMeshAgent = _componentHolder.GetComponent<NavMeshAgent>();
        }

        public void DeInitialize()
        {
        }

        public void StartTracking()
        {
            DeckEventManager.Register<DeckOnNavMeshPositionSelectionEvent>(SetDestination);
        }

        public void StopTracking()
        {
            DeckEventManager.Unregister<DeckOnNavMeshPositionSelectionEvent>(SetDestination);
        }

        public void Tick()
        {
        }

        public DeckComponentHolder GetComponentOwner()
        {
            return _componentHolder;
        }


        public void SetDestination(DeckOnNavMeshPositionSelectionEvent obj)
        {
            _navMeshAgent.SetDestination(obj.position);
        }


        public object GetData()
        {
            return new DeckMovementComponentData()
            {
                position = _componentHolder.transform.position,
                rotation = _componentHolder.transform.rotation.eulerAngles
            };
        }

        public void LoadData(string value)
        {
            var data = JsonUtility.FromJson<DeckMovementComponentData>(value);
            _navMeshAgent.Warp(data.position);
            _componentHolder.transform.rotation = Quaternion.Euler(data.rotation);
        }

        [Serializable]
        public class DeckMovementComponentData
        {
            public Vector3 position;
            public Vector3 rotation;
        }
    }
}