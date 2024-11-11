using Deck.Components;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Utility;

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
            DeckEventManager.Register<DeckEventOnCoreAgentDeath>(OnSelectableDeath);
            DeckEventManager.Register<DeckEventOnCoreAgentCreated>(OnCoreAgentCreated);
        }

        public override void DeInitialize()
        {
            _hotKeySelectionHandler.DeInitialize();
            DeckEventManager.Unregister<DeckEventOnCoreAgentDeath>(OnSelectableDeath);
            DeckEventManager.Unregister<DeckEventOnCoreAgentCreated>(OnCoreAgentCreated);
        }

        public void OnSelection(DeckAgent selection)
        {
            if (selection == currentSelection)
            {
                return;
            }

            OnSelectableClear();
            currentSelection = selection;
            DeckEventOnAgentSelected.Create(currentSelection).Send();
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
                DeckEventOnAgentReleased.Create(currentPossession).Send();
                new DeckCommandRelease(currentPossession).ProcessCommand(default);
            }

            currentPossession = possession;
            new DeckCommandPossess(currentPossession).ProcessCommand(default);
            DeckEventOnAgentPossessed.Crate(currentPossession).Send();
        }

        private void Update()
        {
            _hotKeySelectionHandler.Tick();
        }

        public void OnSelectableDeath(DeckEventOnCoreAgentDeath dead)
        {
            if (dead.agent == currentSelection)
            {
                currentSelection = null;
                DeckEventOnSelectionReleased.Create(currentSelection).Send();
            }

            if (dead.agent == currentPossession)
            {
                currentPossession = null;
                DeckEventOnAgentReleased.Create(currentSelection).Send();
            }
        }

        public static void ResetSelectionToPossession()
        {
            currentSelection = currentPossession;

            if (currentPossession == null)
            {
                DeckEventOnAgentReleased.Create(null).Send();
            }
            else
            {
                DeckEventOnAgentSelected.Create(currentPossession).Send();
            }
        }

        public void OnSelectableClear()
        {
            currentSelection = null;
            DeckEventOnSelectionReleased.Create(currentSelection).Send();
        }


        private void OnCoreAgentCreated(DeckEventOnCoreAgentCreated obj)
        {
            if (currentPossession == null)
            {
                OnSelection(obj.agent);
                OnPossession(obj.agent);
            }
        }
    }
}