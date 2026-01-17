using System;
using Base;
using Cysharp.Threading.Tasks;
using Data.Component;
using Systems.SystemSave;
using UI.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace Components.Movement
{
    [Serializable]
    public class DeckComponentMovement : DeckComponent
    {
        private const string MOVEMENT_SPEED_STAT_NAME = "Movement Speed";
        private const string MOVEMENT_SPEED_STAT_DESCRIPTION = "Agents movement speed towards a target";

        [SerializeField] private DeckDataMovement movementData;

        private NavMeshAgent _navMeshAgent;
        private bool _hasInitialized;
        private bool _static;

        protected override void InternalPreInitialize()
        {
            if (_hasInitialized)
            {
                return;
            }

            _hasInitialized = true;

            if (!agent.TryGetComponent<NavMeshAgent>(out var result))
            {
                _static = true;
                return;
            }

            _navMeshAgent = result;
            SetOriginalSpeed();
        }

        public void SetOriginalSpeed()
        {
            if (_static || movementData == null)
            {
                return;
            }

            _navMeshAgent.speed = movementData.MovementSpeed;
            _navMeshAgent.acceleration = movementData.Acceleration;
            _navMeshAgent.angularSpeed = movementData.AngularSpeed;
            _navMeshAgent.stoppingDistance = movementData.StoppingDistance;
        }

        public void SetModifiedSpeed(float newSpeed = -1f, float newAcceleration = -1f, float newAngularSpeed = -1f)
        {
            InternalPreInitialize();
            if (newSpeed > 0f)
            {
                _navMeshAgent.speed = newSpeed;
            }

            if (newAcceleration > 0)
            {
                _navMeshAgent.acceleration = newAcceleration;
            }

            if (newAngularSpeed > 0)
            {
                _navMeshAgent.angularSpeed = newAngularSpeed;
            }
        }

        public bool SetDestination(Vector3 target, float desiredDistance = 0f)
        {
            if (!_static)
            {
                if (!_navMeshAgent.isOnNavMesh)
                {
                    return true;
                }

                _navMeshAgent.SetDestination(target);
            }

            var distance = Vector3.Distance(agent.transform.position, _navMeshAgent.destination);
            return distance > desiredDistance;
        }

        public async void InterruptMovement(int cancellationDelay = 5000)
        {
            SetDestination(_navMeshAgent.transform.position);
            SetMovementStatus(false);
            await UniTask.Delay(cancellationDelay);
            SetMovementStatus(true);
        }

        public void ContinueMovement()
        {
            SetMovementStatus(true);
        }

        private void SetMovementStatus(bool newStatus)
        {
            if (newStatus)
            {
                _navMeshAgent.speed = movementData.MovementSpeed;
            }
            else
            {
                _navMeshAgent.speed = 0;
            }
        }

        public override object GetData()
        {
            var transform1 = agent.transform;

            return new DeckMovementComponentData
            {
                position = transform1.position,
                rotation = transform1.rotation,
                velocity = _static ? Vector3.zero : _navMeshAgent.velocity,
                targetPosition = _static ? Vector3.zero : _navMeshAgent.destination
            };
        }

        public override void LoadData(string value)
        {
            var data = DeckSaveUtility.GetDeserializedData<DeckMovementComponentData>(value);

            if (_static)
            {
                var transform1 = agent.transform;
                transform1.position = data.position;
                transform1.rotation = data.rotation;
                return;
            }

            _navMeshAgent.Warp(data.position);
            _navMeshAgent.destination = data.targetPosition;

            _navMeshAgent.velocity = data.velocity;
            agent.transform.rotation = data.rotation;
        }

        public float GetSpeed()
        {
            if (_static)
            {
                return 0;
            }

            return _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
        }

        public override DeckStatGroup GetStatGroup()
        {
            if (_static)
            {
                return new DeckStatGroup();
            }

            return new DeckStatGroup(new[]
            {
                new DeckStat(MOVEMENT_SPEED_STAT_NAME, movementData.MovementSpeed.ToString(), MOVEMENT_SPEED_STAT_DESCRIPTION)
            }, this);
        }

        public bool ReachedToDestination()
        {
            return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
        }

        public void SetRotation(Quaternion rotation)
        {
            _navMeshAgent.transform.rotation = rotation;
        }

        public void SetPosition(Vector3 position)
        {
            _navMeshAgent.transform.position = position;
            _navMeshAgent.Warp(position);
        }

        public void SetDisabled()
        {
            _navMeshAgent.enabled = false;
        }

        public void SetEnabled()
        {
            _navMeshAgent.enabled = true;
        }

        [Serializable]
        public struct DeckMovementComponentData
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 targetPosition;
            public Vector3 velocity;
        }
    }
}