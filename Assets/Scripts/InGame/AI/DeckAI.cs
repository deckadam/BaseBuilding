using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Utility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Deck.InGame.AI
{
    public class DeckAI : MonoBehaviour
    {
        [SerializeField] protected SerializedDictionary<DeckAITrigger, IDeckAIState> stateDictionary;
        [SerializeField] protected DeckAITrigger initialTrigger;
        protected DeckAgent AgentToControl;
        protected CancellationTokenSource TokenSource;
        protected IDeckAIState ActiveState;

        public void StartAi()
        {
            AgentToControl = GetComponent<DeckAgent>();
            stateDictionary = new SerializedDictionary<DeckAITrigger, IDeckAIState>();
            var states = GetComponentsInChildren<IDeckAIState>();
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