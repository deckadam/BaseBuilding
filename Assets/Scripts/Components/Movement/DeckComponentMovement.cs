using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data.Component;
using UnityEngine;
using UnityEngine.AI;

namespace Deck.Components
{
    [Serializable]
    public class DeckComponentMovement : DeckComponent
    {
        private NavMeshAgent _navMeshAgent;
        private DeckDataMovement _movementData;
        private bool _static;
        private CancellationTokenSource _interruptCancellation;

        protected override void InternalPreInitialize()
        {
            if (!holder.TryGetComponent<NavMeshAgent>(out var result))
            {
                _static = true;
                return;
            }

            _navMeshAgent = result;
            _movementData = holder.GetData<DeckDataMovement>();
            _navMeshAgent.speed = _movementData.MovementSpeed;
            _navMeshAgent.acceleration = _movementData.Acceleration;
            _navMeshAgent.angularSpeed = _movementData.AngularSpeed;
        }

        public bool SetDestination(Vector3 target, float desiredDistance = 0f)
        {
            if (!_static)
            {
                _navMeshAgent.SetDestination(target);
            }

            var distance = Vector3.Distance(holder.transform.position, target);
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
                _navMeshAgent.speed = _movementData.MovementSpeed;
            }
            else
            {
                _navMeshAgent.speed = 0;
            }
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

        public float GetSpeed()
        {
            if (_static)
            {
                return 0;
            }

            return _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
        }

        [Serializable]
        public class DeckMovementComponentData
        {
            public Vector3 position;
            public Vector3 rotation;
        }
    }
}