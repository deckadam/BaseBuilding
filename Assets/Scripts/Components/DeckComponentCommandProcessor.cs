using System.Collections.Generic;
using System.Threading;
using Base;
using Commands;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Utility;

namespace Components
{
    public class DeckComponentCommandProcessor : DeckComponent
    {
        private Queue<DeckCommand> _waitingCommands = new();
        private CancellationTokenSource _taskExecutionTokenSource;
        private DeckCommand _activeCommand;
        private bool _waiting;
        private DeckAgentHumanoid _humanoid;
        public bool _hasActiveCommand = false;

        protected override void InternalPostInitialize()
        {
            _humanoid = GetComponent<DeckAgentHumanoid>();
        }

        public void StopExecutions()
        {
            _taskExecutionTokenSource?.Cancel();
            _taskExecutionTokenSource?.Dispose();
            _taskExecutionTokenSource = null;
        }

        public async void StartProcessingCommands()
        {
            _taskExecutionTokenSource = new CancellationTokenSource();
            var destroyToken = gameObject.GetCancellationTokenOnDestroy();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_taskExecutionTokenSource.Token, destroyToken);
            while (!linkedTokenSource.IsCancellationRequested)
            {
                if (_waitingCommands.Count > 0)
                {
                    _waiting = false;
                    _hasActiveCommand = true;
                    var localRef = _waitingCommands.Dequeue();
                    _activeCommand = localRef;

                    var status = await localRef.ProcessCommand(linkedTokenSource.Token).SuppressCancellationThrow();

                    if (!status.IsCanceled)
                    {
                        localRef.OnCompleted?.Invoke();
                        localRef.OnCompletedWithAgent?.Invoke(agent);
                    }

                    _hasActiveCommand = false;
                    _activeCommand = null;
                }
                else if (!_waiting)
                {
                    _waiting = true;
                    _humanoid.OnWaiting();
                }

                await UniTask.Yield();
            }
        }

        public void AddCommand(DeckCommand command, bool isInterrupting = false)
        {
            if (isInterrupting)
            {
                InterruptQueue(command);
            }
            else
            {
                _waitingCommands.Enqueue(command);
            }
        }

        public void EnqueueCommand(DeckCommand command)
        {
            _waitingCommands.Enqueue(command);
        }

        private void InterruptQueue(DeckCommand command)
        {
            _waitingCommands.Clear();
            _taskExecutionTokenSource?.Cancel();
            _taskExecutionTokenSource?.Dispose();
            _taskExecutionTokenSource = new CancellationTokenSource();
            _waitingCommands.Enqueue(command);
            StartProcessingCommands();
        }

        public string GetCurrentCommandSaveData()
        {
            return _activeCommand.GetSaveData();
        }
    }
}