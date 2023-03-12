using System;
using Data.Component;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

namespace Deck.Components
{
    [Serializable]
    public class DeckMovementComponent : DeckComponent
    {
        private NavMeshAgent _navMeshAgent;

        protected override void Initialize()
        {
            _navMeshAgent = holder.gameObject.AddComponent<NavMeshAgent>();
            
            var data = holder.GetData<DeckDataMovement>();
            _navMeshAgent.speed = data.MovementSpeed;
            _navMeshAgent.acceleration = data.Acceleration;
            _navMeshAgent.angularSpeed = data.AngularSpeed;
        }

        public override void DeInitialize()
        {
            Object.Destroy(_navMeshAgent);
        }

        public bool SetDestination(Vector3 target)
        {
            _navMeshAgent.SetDestination(target);

            var distance = Vector3.Distance(_navMeshAgent.transform.position, target);

            if (distance < 1f)
            {
                return true;
            }

            return false;
        }

        public override object GetData()
        {
            return new DeckMovementComponentData
            {
                position = holder.transform.position,
                rotation = holder.transform.rotation.eulerAngles
            };
        }

        public override void LoadData(string value)
        {
            var data = JsonUtility.FromJson<DeckMovementComponentData>(value);
            _navMeshAgent.Warp(data.position);
            holder.transform.rotation = Quaternion.Euler(data.rotation);
        }

        [Serializable]
        public class DeckMovementComponentData
        {
            public Vector3 position;
            public Vector3 rotation;
        }
    }
}