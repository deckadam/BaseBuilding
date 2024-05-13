using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Agent;
using Deck.UI.InGame.AI.Enemy;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.Rendering;

namespace Deck.UI.InGame.AI
{
    public class DeckAI : MonoBehaviour
    {
        [SerializeField] protected SerializedDictionary<DeckAITrigger, DeckAIState> stateDictionary;
        [SerializeField] protected DeckAITrigger initialTrigger;
        protected DeckAgent AgentToControl;
        protected CancellationTokenSource TokenSource;
        protected DeckAIState ActiveState;

        public void StartAi()
        {
            AgentToControl = GetComponent<DeckAgent>();
            stateDictionary = new SerializedDictionary<DeckAITrigger, DeckAIState>();
            var states = GetComponentsInChildren<DeckAIState>();
            foreach (var deckAIState in states)
            {
                stateDictionary.Add(deckAIState.GetTrigger(), deckAIState);
            }  
            foreach (var deckAIState in stateDictionary.Values)
            {
                deckAIState.SetAgent(AgentToControl);
            }

            StartBehaviour(initialTrigger);
        }

        private void StartBehaviour(DeckAITrigger trigger)
        {
            if (stateDictionary.TryGetValue(trigger, out ActiveState))
            {
                TokenSource = new CancellationTokenSource();
                ActiveState.OnStateRequest(TokenSource).Forget();
            }
            else
            {
                DeckLogger.Error("DeckAi StartBehaviour Initial trigger not found in dictionary.");
            }
        }
    }
}