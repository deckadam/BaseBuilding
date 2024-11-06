using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components.Building.Stats;
using Deck.Data.Component;
using Deck.Save;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentMovement : DeckComponent
    {
        private const string MOVEMENT_SPEED_STAT_NAME = "Movement Speed";
        private const string MOVEMENT_SPEED_STAT_DESCRIPTION = "The speed at which the agent moves";

        [SerializeField] private DeckDataMovement movementData;
        private NavMeshAgent _navMeshAgent;
        private bool _static;
        private CancellationTokenSource _interruptCancellation;

        private bool _hasInitiailized;

        protected override void InternalPreInitialize()
        {
            if (_hasInitiailized)
            {
                return;
            }

            _hasInitiailized = true;

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
            if (_navMeshAgent == null || movementData == null)
            {
                return;
            }

            _navMeshAgent.speed = movementData.MovementSpeed;
            _navMeshAgent.acceleration = movementData.Acceleration;
            _navMeshAgent.angularSpeed = movementData.AngularSpeed;
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
            _interruptCancellation?.Cancel();
            _interruptCancellation?.Dispose();
            _interruptCancellation = new CancellationTokenSource();

            SetMovementStatus(false);
            await UniTask.Delay(cancellationDelay, cancellationToken: _interruptCancellation.Token).SuppressCancellationThrow();
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

        [Serializable]
        public class DeckMovementComponentData
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 targetPosition;
            public Vector3 velocity;
        }
    }
}