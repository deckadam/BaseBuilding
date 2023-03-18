using System.Linq;
using Deck.EventManager;
using Deck.Services.Implementations.CellSelectionService.Events;
using Deck.Services.Implementations.MapService;
using Deck.Utility.Constants.Hotkey.Events;
using Deck.Utility.Logger;
using ModestTree;
using UnityEngine;

namespace Deck.Services.Implementations.CellSelectionService
{
    public class DeckHotKeySelectionHandler
    {
        private DeckSelectionService _selectionService;
        private DeckMapService _mapService;

        public void Initialize()
        {
            _mapService = Deck.GetService<DeckMapService>();
            _selectionService = Deck.GetService<DeckSelectionService>();
            DeckEventManager.Register<DeckOnAgentPossessedEvent>(OnAgentPossessed);
        }

        public void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnAgentPossessedEvent>(OnAgentPossessed);
        }

        private void OnAgentPossessed(DeckOnAgentPossessedEvent obj)
        {
            var agents = _mapService.GetAgents().ToArray();
            var index = agents.IndexOf(obj.agent);
            if (index == -1)
            {
                return;
            }

            SelectAgentByIndex(index);
        }

        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                SelectAgentByIndex(9);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectAgentByIndex(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectAgentByIndex(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectAgentByIndex(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SelectAgentByIndex(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SelectAgentByIndex(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SelectAgentByIndex(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                SelectAgentByIndex(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                SelectAgentByIndex(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                SelectAgentByIndex(8);
            }
        }

        private void SelectAgentByIndex(int index)
        {
            var agents = _mapService.GetAgents().ToArray();
            if (agents.Length <= index)
            {
                return;
            }

            DeckOnHotkeySelected.Create(index).Send();
            var agentToPossess = agents[index];
            _selectionService.OnPossession(agentToPossess);
        }
    }
}