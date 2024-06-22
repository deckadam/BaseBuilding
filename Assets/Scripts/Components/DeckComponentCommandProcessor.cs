using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Save;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Commands
{
    public class DeckComponentCommandProcessor : DeckComponent
    {
        private Queue<DeckCommand> _waitingCommands = new();
        private CancellationTokenSource _selfExecutionToken;
        private CancellationTokenSource _taskExecutionTokenSource;
        private DeckCommand _activeCommand;

        public void StopExecutions()
        {
            _selfExecutionToken.Cancel();
            _selfExecutionToken.Dispose();
        }

        public async void StartProcessCommands()
        {
            _selfExecutionToken = new CancellationTokenSource();
            _taskExecutionTokenSource = new CancellationTokenSource();
            while (!_selfExecutionToken.IsCancellationRequested)
            {
                if (_waitingCommands.Count > 0)
                {
                    _activeCommand = _waitingCommands.Dequeue();
                    var status = await _activeCommand.ProcessCommand(_taskExecutionTokenSource.Token).SuppressCancellationThrow();
                    _activeCommand = null;
                    if (status.IsCanceled)
                    {
                        DeckLogger.Command("Canceled");
                    }
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

        public void EnqueCommand(DeckCommand command)
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