using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Agent;
using Deck.Commands;
using Deck.UI.InGame.AI;

namespace InGame.AI.Enemy
{
    public class DeckAiEnemy : DeckAI
    {
        private void Start()
        {
            AgentToControl.GetComponentInParent<DeckComponentHealth>().OnDamageTaken += TransitionToRunning;
            TokenSource = new CancellationTokenSource();
        }

        private async void TransitionToRunning(DeckAgent damageDealer)
        {
            TokenSource?.Cancel();
            TokenSource?.Dispose();
            TokenSource = new CancellationTokenSource();

            if (AgentToControl == null)
            {
                return;
            }

            ActiveState = stateDictionary[DeckAITrigger.RunAway];
            await ActiveState.OnStateRequest(TokenSource);

            stateDictionary[DeckAITrigger.Wander].OnStateRequest(new CancellationTokenSource(), damageDealer).Forget();
        }
    }
}