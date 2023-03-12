using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components.Operations;

namespace Components
{
    public class DeckAgentCommandProcessor
    {
        private Queue<DeckCommand> _waitingCommands;
        private CancellationTokenSource _tokenSource;

        public DeckAgentCommandProcessor()
        {
            _waitingCommands = new Queue<DeckCommand>();
            _tokenSource = new CancellationTokenSource();
            ProcessCommands(_tokenSource.Token);
        }

        public void StopExecutions()
        {
            _tokenSource.Cancel();
        }

        private async void ProcessCommands(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_waitingCommands.Count > 0)
                {
                    var _dequeuedCommand = _waitingCommands.Dequeue();
                    await _dequeuedCommand.ProcessCommand();
                }

                await UniTask.Yield();
            }
        }

        public void AddCommand(DeckCommand command)
        {
            _waitingCommands.Enqueue(command);
        }
    }
}