using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Deck.Components
{
    public class DeckCommandProcessor
    {
        private Queue<DeckCommand> _waitingCommands;
        private CancellationTokenSource _selfExecutionToken;
        private CancellationTokenSource _taskExecutionTokenSource;

        public DeckCommandProcessor()
        {
            _waitingCommands = new Queue<DeckCommand>();
            _selfExecutionToken = new CancellationTokenSource();
            ProcessCommands(_selfExecutionToken.Token);
        }

        public void StopExecutions()
        {
            _selfExecutionToken.Cancel();
            _selfExecutionToken.Dispose();
        }

        private async void ProcessCommands(CancellationToken token)
        {
            var isCrashed = false;
            try
            {
                _taskExecutionTokenSource = new CancellationTokenSource();
                while (!token.IsCancellationRequested)
                {
                    if (_waitingCommands.Count > 0)
                    {
                        var _dequeuedCommand = _waitingCommands.Dequeue();
                        await _dequeuedCommand.ProcessCommand(_taskExecutionTokenSource.Token).SuppressCancellationThrow();
                    }

                    await UniTask.Yield();
                }
            }
            catch (Exception e)
            {
                isCrashed = true;
                Debug.LogError(e.Message);
            }
            finally
            {
                if (isCrashed)
                {
                    _selfExecutionToken = new CancellationTokenSource();
                    ProcessCommands(_selfExecutionToken.Token);
                }
            }
        }

        public void AddCommand(DeckCommand command)
        {
            if (command.InterrupintgCommand())
            {
                InterruptQueue(command);
            }
            else
            {
                _waitingCommands.Enqueue(command);
            }
        }

        private void InterruptQueue(DeckCommand command)
        {
            _waitingCommands.Clear();
            _taskExecutionTokenSource?.Cancel();
            _taskExecutionTokenSource?.Dispose();
            _taskExecutionTokenSource = new CancellationTokenSource();
            _waitingCommands.Enqueue(command);
        }
    }
}