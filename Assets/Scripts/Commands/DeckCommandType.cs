using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Deck.Commands
{
    public class DeckCommand
    {
        public virtual UniTask<bool> ProcessCommand(CancellationToken token)
        {
            return default;
        }

        public virtual void OnInterrupt()
        {
        }

        public virtual string GetSaveData()
        {
            return string.Empty;
        }

        public virtual void LoadSaveData(string saveData)
        {
        }
    }
}