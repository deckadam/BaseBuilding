using System.Threading;
using Base;
using Components.Movement;
using Cysharp.Threading.Tasks;
using Data.Component;
using UnityEngine;
using Utility;

namespace InGame.AI.States
{
    public class DeckAIWander : MonoBehaviour, IDeckAIState
    {
        [SerializeField] private DeckDataMovement wanderingSpeed;
        [SerializeField] private float wanderingRadius;
        [SerializeField] private int wanderDelay;

        private DeckAgent _agentToControl;
        private Vector3 _originPosition;

        public async UniTask OnStateRequest(CancellationTokenSource source, params object[] args)
        {
            var componentMovement = _agentToControl.GetDeckComponent<DeckComponentMovement>();

            if (componentMovement == null)
            {
                DeckLogger.Component("DeckAIWander: No movement component found.");
                return;
            }

            componentMovement.SetModifiedSpeed(wanderingSpeed.MovementSpeed);
            var destroyToken = gameObject.GetCancellationTokenOnDestroy();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(source.Token, destroyToken);

            while (!source.IsCancellationRequested)
            {
                var targetPosition = GetWanderingPosition();
                var isCanceled = await UniTask.WaitWhile(() => componentMovement.SetDestination(targetPosition, 0.5f), cancellationToken: linkedTokenSource.Token).SuppressCancellationThrow();
                if (isCanceled)
                {
                    return;
                }

                await UniTask.Delay(wanderDelay);
            }
        }

        public void SetAgent(DeckAgent agentToControl)
        {
            _agentToControl = agentToControl;
            _originPosition = _agentToControl.transform.position;
        }

        private Vector3 GetWanderingPosition()
        {
            var randomPosition = Random.insideUnitSphere * wanderingRadius;
            randomPosition.y = 0;
            return _originPosition + randomPosition;
        }

        public DeckAITrigger GetTrigger() => DeckAITrigger.Wander;
        public bool CanReleaseState() => true;
    }
}