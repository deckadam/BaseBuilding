using System.Threading;
using Base;
using Cysharp.Threading.Tasks;

namespace InGame.AI
{
    public interface IDeckAIState
    {
        UniTask OnStateRequest(CancellationTokenSource source, params object[] args);
        void SetAgent(DeckAgent agent);
        DeckAITrigger GetTrigger();
        bool CanReleaseState();
    }
}