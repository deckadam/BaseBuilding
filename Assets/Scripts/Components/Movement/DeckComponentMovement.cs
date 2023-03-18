using System;
using Data.Component;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentMovement : DeckComponent
    {
        private NavMeshAgent _navMeshAgent;
        private bool _static;

        protected override void Initialize()
        {
            if (!holder.TryGetComponent<NavMeshAgent>(out var result))
            {
                _static = true;
                return;
            }

            _navMeshAgent = result;
            var data = holder.GetData<DeckDataMovement>();
            _navMeshAgent.speed = data.MovementSpeed;
            _navMeshAgent.acceleration = data.Acceleration;
            _navMeshAgent.angularSpeed = data.AngularSpeed;
        }

        public bool SetDestination(Vector3 target)
        {
            if (!_static)
            {
                _navMeshAgent.SetDestination(target);
            }

            var distance = Vector3.Distance(holder.transform.position, target);
            return distance < 1f;
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

            if (_static)
            {
                holder.transform.position = data.position;
                holder.transform.rotation = Quaternion.Euler(data.rotation);
                return;
            }

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