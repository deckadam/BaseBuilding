using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;

namespace Deck.InGame.AI
{
    public interface IDeckAIState
    {
        UniTask OnStateRequest(CancellationTokenSource source, params object[] args);
        void SetAgent(DeckAgent agent);
        DeckAITrigger GetTrigger();
        bool CanReleaseState();
    }
}