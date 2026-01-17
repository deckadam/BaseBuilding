using System.Threading;
using Base;
using Components.Movement;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Data.Component;
using UnityEngine;
using Utility;

namespace InGame.AI.States
{
    public class DeckAIRunAway : MonoBehaviour, IDeckAIState
    {
        [SerializeField] private DeckDataMovement runningSpeed;
        [SerializeField] private int runDuration;

        private Vector3 _originPosition;
        private DeckAgent _agentToControl;

        public void SetAgent(DeckAgent agentToControl)
        {
            _agentToControl = agentToControl;
            _originPosition = _agentToControl.transform.position;
        }

        public async UniTask OnStateRequest(CancellationTokenSource tokenSource, params object[] args)
        {
            if (args == null || args.Length == 0 || args[0] is not DeckAgent damageDealer)
            {
                DeckLogger.Component("DeckAIRunAway: No damage dealer found.");
                return;
            }

            var componentMovement = _agentToControl.GetDeckComponent<DeckComponentMovement>();

            if (componentMovement == null)
            {
                DeckLogger.Component("DeckAIRunAway: No movement component found.");
                return;
            }

            componentMovement.SetModifiedSpeed(runningSpeed.MovementSpeed);

            var source = _agentToControl.gameObject.GetAsyncCancelTrigger().GetCancellationSourceTokenOnDestroy();
            source.CancelAfterSlim(runDuration);

            while (!source.IsCancellationRequested)
            {
                var targetPosition = GetRunningAwayPosition(damageDealer);
                var isCanceled = await UniTask.WaitWhile(() => componentMovement.SetDestination(targetPosition, 0.5f), cancellationToken: source.Token).SuppressCancellationThrow();
                if (isCanceled)
                {
                    return;
                }
            }
        }

        private Vector3 GetRunningAwayPosition(DeckAgent damageDealer)
        {
            var direction = transform.position - damageDealer.transform.position;
            direction = direction.normalized;
            return transform.position + direction * 20;
        }

        public DeckAITrigger GetTrigger() => DeckAITrigger.RunAway;
        public bool CanReleaseState() => true;
    }
}