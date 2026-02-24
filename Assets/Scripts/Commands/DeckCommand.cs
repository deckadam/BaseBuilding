using System;
using System.Threading;
using Base;
using Cysharp.Threading.Tasks;

namespace Commands
{
    public class DeckCommand
    {
        public Action OnCompleted;
        public Action<DeckAgent> OnCompletedWithAgent;

        protected DeckCommand()
        {
        }

        public DeckCommand RegisterToOnCompleted(Action onCompleted)
        {
            OnCompleted += onCompleted;
            return this;
        }

        public DeckCommand RegisterToOnCompleted(Action<DeckAgent> onCompleted)
        {
            OnCompletedWithAgent += onCompleted;
            return this;
        }

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