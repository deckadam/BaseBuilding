using System;
using System.Globalization;
using Base;
using Cysharp.Threading.Tasks;
using Data.Agent;
using Systems.SystemSave;
using UI.Stats;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Components.Movement
{
    [Serializable]
    public class DeckComponentMovement : DeckComponent
    {
        private const string MOVEMENT_SPEED_STAT_NAME = "Movement Speed";
        private const string MOVEMENT_SPEED_STAT_DESCRIPTION = "Agents movement speed towards a target";

        private DeckDataMovement _movementData;

        private Action onDestinationReached;

        private NavMeshAgent _navMeshAgent;
        private bool _hasInitialized;
        private bool _static;
        private bool _hasDestination;

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
            _movementData = agent.GetAgentData<DeckDataMovement>();
            SetOriginalSpeed();
        }

        public void SetOriginalSpeed()
        {
            if (_static || _movementData == null)
            {
                return;
            }

            _navMeshAgent.speed = _movementData.MovementSpeed;
            _navMeshAgent.acceleration = _movementData.Acceleration;
            _navMeshAgent.angularSpeed = _movementData.AngularSpeed;
            _navMeshAgent.stoppingDistance = _movementData.StoppingDistance;
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

        public bool SetDestination(Vector3 target, Action onDestinationReached = null, float desiredDistance = 0f)
        {
            if (!_static)
            {
                if (!_navMeshAgent.isOnNavMesh)
                {
                    return true;
                }

                _navMeshAgent.SetDestination(target);
            }

            _hasDestination = true;
            this.onDestinationReached = onDestinationReached;
            var distance = Vector3.Distance(agent.GetPosition(), _navMeshAgent.destination);
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
                _navMeshAgent.speed = _movementData.MovementSpeed;
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
                new DeckStat(MOVEMENT_SPEED_STAT_NAME, _movementData.MovementSpeed.ToString(CultureInfo.InvariantCulture), MOVEMENT_SPEED_STAT_DESCRIPTION)
            }, this);
        }

        public bool ReachedToDestination()
        {
            return Vector3.Distance(_navMeshAgent.destination, transform.position) <= _navMeshAgent.stoppingDistance;
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
            _hasDestination = false;
        }

        public void SetEnabled()
        {
            _navMeshAgent.enabled = true;
            _hasDestination = false;
        }

        private void Update()
        {
            if (!_hasDestination) return;
            if (!(Vector3.Distance(transform.position, _navMeshAgent.destination) < _movementData.StoppingDistance)) return;

            onDestinationReached?.Invoke();
            onDestinationReached = null;
            _hasDestination = false;
        }

        public bool PathPending()
        {
            return _navMeshAgent.pathPending || _navMeshAgent.pathStatus != NavMeshPathStatus.PathComplete;
        }

        public void Warp(Vector3 position)
        {
            _navMeshAgent.Warp(position);
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