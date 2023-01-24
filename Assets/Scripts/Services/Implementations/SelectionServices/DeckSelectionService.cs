using Deck.Component;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Map;
using Deck.Map.Selection;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Utility;
using Deck.Utility.Logger;
using UnityEngine;
using Utility;
using Zenject;

namespace Deck.Services.Implementations.CellSelectionService
{
    public class DeckSelectionService : DeckServiceBase
    {
        public static DeckComponentHolder currentSelection { get; private set; }
        public static DeckComponentHolder currentPossession { get; private set; }

        private DeckCell _currentDeckCell;
        private Texture2D _selectionTexture;
        private Material _groundMaterial;
        private Vector2Int _size;
        private DeckHotKeySelectionHandler _hotKeySelectionHandler;

        private DeckSelectionHighlighter _highlighter;

        [Inject]
        private void Inject(DeckSelectionHighlighter highlighter)
        {
            _highlighter = highlighter;
        }

        public override void Initialize()
        {
            _hotKeySelectionHandler = new DeckHotKeySelectionHandler();
            DeckEventManager.Register<DeckOnCellClickedEvent>(OnCellClicked);
            DeckEventManager.Register<DeckOnComponentHolderDeath>(OnSelectableDeath);
            DeckEventManager.Register<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnCellClickedEvent>(OnCellClicked);
            DeckEventManager.Unregister<DeckOnComponentHolderDeath>(OnSelectableDeath);
            DeckEventManager.Unregister<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public void SetMapData(Vector2Int size, Material groundMaterial)
        {
            _size = size;
            _selectionTexture = DeckUtility.GenerateTexture(size);
            _groundMaterial = groundMaterial;
            _groundMaterial.SetTexture(DeckShaderConstants.SelectionTexture, _selectionTexture);
        }

        private void OnCellClicked(DeckOnCellClickedEvent obj)
        {
            _currentDeckCell = obj.deckCell;
            _selectionTexture.SetPixel(_size.x - obj.deckCell.cellIndex.x, _size.y - obj.deckCell.cellIndex.y, Color.red);
            _selectionTexture.Apply();
        }


        public void OnSelection(DeckComponentHolder selection)
        {
            if (selection == currentSelection)
            {
                return;
            }

            OnSelectableClear();
            currentSelection = selection;
            _highlighter.SetTarget(selection);
        }

        public void OnPossession(DeckComponentHolder possession)
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

            currentPossession?.OnPossessionEnd();
            currentPossession = possession;
            currentPossession.OnPossessionStart();
        }

        private void Update()
        {
            _hotKeySelectionHandler.Tick();
        }

        public void OnSelectableDeath(DeckOnComponentHolderDeath dead)
        {
            if (dead.componentHolder == currentSelection)
            {
                currentSelection = null;
                _highlighter.ClearTarget();
            }

            if (dead.componentHolder == currentPossession)
            {
                currentPossession = null;
                _highlighter.ClearTarget();
            }
        }

        public void ResetSelectionToPossession()
        {
            currentSelection = currentPossession;
            _highlighter.SetTarget(currentPossession);
        }

        public void OnSelectableClear()
        {
            currentSelection = null;
            _highlighter.ClearTarget();
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