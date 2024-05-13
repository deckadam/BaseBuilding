using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Agent;

namespace Deck.UI.InGame.AI.Enemy
{
    public interface DeckAIState
    {
        UniTask OnStateRequest(CancellationTokenSource source, params object[] args);
        void SetAgent(DeckAgent agent);
        DeckAITrigger GetTrigger();
        bool CanReleaseState();
    }
}