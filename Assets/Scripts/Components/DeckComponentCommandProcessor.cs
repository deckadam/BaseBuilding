using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Commands;
using Deck.Save;
using Deck.Utility;

namespace Deck.Components
{
    public class DeckComponentCommandProcessor : DeckComponent
    {
        private Queue<DeckCommand> _waitingCommands = new();
        private CancellationTokenSource _taskExecutionTokenSource;
        private DeckCommand _activeCommand;
        private bool _waiting;
        private DeckAgentHumanoid _humanoid;

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

        public async void StartProcessCommands()
        {
            _taskExecutionTokenSource = new CancellationTokenSource();
            var destroyToken = gameObject.GetCancellationTokenOnDestroy();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_taskExecutionTokenSource.Token, destroyToken);
            while (!linkedTokenSource.IsCancellationRequested)
            {
                if (_waitingCommands.Count > 0)
                {
                    _waiting = false;
                    _activeCommand = _waitingCommands.Dequeue();
                    var status = await _activeCommand.ProcessCommand(linkedTokenSource.Token).SuppressCancellationThrow();

                    if (status.IsCanceled)
                    {
                        DeckLogger.Command("Canceled");
                    }
                    else
                    {
                        _activeCommand.OnCompleted?.Invoke();
                    }

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

        public void AddCommand(DeckCommand command, bool isInterrupting)
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
        }

        public override object GetData()
        {
            var hasActiveCommand = _activeCommand != null;
            var commandCount = hasActiveCommand ? _waitingCommands.Count + 1 : _waitingCommands.Count;

            var commandDatas = new string[commandCount];
            var commandTypes = new string[commandCount];

            if (hasActiveCommand)
            {
                commandDatas[0] = _activeCommand.GetSaveData();
                commandTypes[0] = _activeCommand.GetType().ToString();
            }

            var lookup = _waitingCommands.ToList();

            var offset = hasActiveCommand ? 1 : 0;
            var totalCount = lookup.Count + offset;
            for (var index = offset; index < totalCount; index++)
            {
                var command = lookup[index];
                commandDatas[index] = command.GetSaveData();
                commandTypes[index] = command.GetType().ToString();
            }

            var data = new SaveData(commandDatas, commandTypes);

            return data;
        }

        public override void LoadData(string value)
        {
            DelayedLoad(value);
        }

        private async void DelayedLoad(string value)
        {
            await UniTask.NextFrame();
            var data = DeckSaveUtility.GetDeserializedData<SaveData>(value);

            for (var index = 0; index < data.commandDatas.Length; index++)
            {
                //Rider beni bi sal be
                var command = (DeckCommand)Activator.CreateInstance(Type.GetType(data.commandTypes[index]) ?? throw new InvalidOperationException());
                command.LoadSaveData(data.commandDatas[index]);
                _waitingCommands.Enqueue(command);
            }
        }

        [Serializable]
        private struct SaveData
        {
            public string[] commandDatas;
            public string[] commandTypes;

            public SaveData(string[] commandDatas, string[] commandTypes)
            {
                this.commandDatas = commandDatas;
                this.commandTypes = commandTypes;
            }
        }
    }
}