using Deck.Component;
using Deck.Components.Operations;
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
        public static DeckAgent currentSelection { get; private set; }
        public static DeckAgent currentPossession { get; private set; }

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
            _hotKeySelectionHandler.Initialize();
            DeckEventManager.Register<DeckOnCellClickedEvent>(OnCellClicked);
            DeckEventManager.Register<DeckOnAgentDeathEvent>(OnSelectableDeath);
            DeckEventManager.Register<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public override void DeInitialize()
        {
            _hotKeySelectionHandler.DeInitialize();
            DeckEventManager.Unregister<DeckOnCellClickedEvent>(OnCellClicked);
            DeckEventManager.Unregister<DeckOnAgentDeathEvent>(OnSelectableDeath);
            DeckEventManager.Unregister<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
        }

        public DeckAgent GetAgentToBeCommanded()
        {
            OnSelection(currentPossession);
            return currentPossession;
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


        public void OnSelection(DeckAgent selection)
        {
            if (selection == currentSelection)
            {
                return;
            }

            OnSelectableClear();
            currentSelection = selection;
            DeckOnAgentSelectedEvent.Create(currentSelection).Send();
            _highlighter.SetTarget(selection);
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
                new DeckCommandRelease(currentPossession).ProcessCommand();
            }

            currentPossession = possession;
            new DeckCommandPossess(currentPossession).ProcessCommand();
            DeckOnAgentPossessedEvent.Crate(currentPossession).Send();
        }

        private void Update()
        {
            _hotKeySelectionHandler.Tick();
        }

        public void OnSelectableDeath(DeckOnAgentDeathEvent dead)
        {
            if (dead.agent == currentSelection)
            {
                currentSelection = null;
                DeckOnSelectionReleasedEvent.Create(currentSelection).Send();
                _highlighter.ClearTarget();
            }

            if (dead.agent == currentPossession)
            {
                currentPossession = null;
                DeckOnAgentReleasedEvent.Create(currentSelection).Send();
                _highlighter.ClearTarget();
            }

            DeckOnAgentDeathEvent.Create(dead.agent).Send();
        }

        public void ResetSelectionToPossession()
        {
            currentSelection = currentPossession;
            _highlighter.SetTarget(currentPossession);
        }

        public void OnSelectableClear()
        {
            currentSelection = null;
            DeckOnSelectionReleasedEvent.Create(currentSelection).Send();
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