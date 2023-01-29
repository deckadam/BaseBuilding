using System;
using Data.Component;
using Deck.Components.Operations;
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
            _navMeshAgent.speed = holder.GetData<DeckDataMovement>().MovementSpeed;
            _navMeshAgent.acceleration = holder.GetData<DeckDataMovement>().Acceleration;
        }

        public override void DeInitialize()
        {
            Object.Destroy(_navMeshAgent);
        }

        private void SetDestination(DeckCommand command)
        {
            var movementCommand = command as DeckCommandMove;
            _navMeshAgent.SetDestination(movementCommand.targetPosition);
        }


        public override DeckCommandListener[] GetSupportedCommandTypes()
        {
            return new[] {DeckCommandListener.Create(DeckCommandType.Move, SetDestination)};
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