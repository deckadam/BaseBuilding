using Deck.Agent;
using Deck.Commands;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Services;
using Deck.Utility.Logger;

namespace Deck.Services.CellSelectionService
{
    public class DeckServiceSelection : DeckServiceBase
    {
        public static DeckAgent currentSelection { get; private set; }
        public static DeckAgent currentPossession { get; private set; }

        private DeckHotKeySelectionHandler _hotKeySelectionHandler;

        public override void Initialize()
        {
            _hotKeySelectionHandler = new DeckHotKeySelectionHandler();
            _hotKeySelectionHandler.Initialize();
            DeckEventManager.Register<DeckOnCoreAgentDeathEvent>(OnSelectableDeath);
            DeckEventManager.Register<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public override void DeInitialize()
        {
            _hotKeySelectionHandler.DeInitialize();
            DeckEventManager.Unregister<DeckOnCoreAgentDeathEvent>(OnSelectableDeath);
            DeckEventManager.Unregister<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public void OnSelection(DeckAgent selection)
        {
            if (selection == currentSelection)
            {
                return;
            }

            OnSelectableClear();
            currentSelection = selection;
            DeckOnAgentSelectedEvent.Create(currentSelection).Send();
        }

        public void OnPossession(DeckAgent possession)
        {
            if (possession == null)
            {
                DeckLogger.Service("On posession on null object");
                return;
            }

            if (possession == currentPossession)
            {
                return;
            }

            currentSelection = possession;

            if (currentPossession != null)
            {
                DeckOnAgentReleasedEvent.Create(currentPossession).Send();
                new DeckCommandRelease(currentPossession).ProcessCommand(default);
            }

            currentPossession = possession;
            new DeckCommandPossess(currentPossession).ProcessCommand(default);
            DeckOnAgentPossessedEvent.Crate(currentPossession).Send();
        }

        private void Update()
        {
            _hotKeySelectionHandler.Tick();
        }

        public void OnSelectableDeath(DeckOnCoreAgentDeathEvent dead)
        {
            if (dead.agent == currentSelection)
            {
                currentSelection = null;
                DeckOnSelectionReleasedEvent.Create(currentSelection).Send();
            }

            if (dead.agent == currentPossession)
            {
                currentPossession = null;
                DeckOnAgentReleasedEvent.Create(currentSelection).Send();
            }
        }

        public static void ResetSelectionToPossession()
        {
            currentSelection = currentPossession;

            if (currentPossession == null)
            {
                DeckOnAgentReleasedEvent.Create(null).Send();
            }
            else
            {
                DeckOnAgentSelectedEvent.Create(currentPossession).Send();
            }
        }

        public void OnSelectableClear()
        {
            currentSelection = null;
            DeckOnSelectionReleasedEvent.Create(currentSelection).Send();
        }


        private void OnCoreAgentCreated(DeckOnCoreAgentCreatedEvent obj)
        {
            if (currentPossession == null)
            {
                OnSelection(obj.agent);
                OnPossession(obj.agent);
            }
        }
    }
}