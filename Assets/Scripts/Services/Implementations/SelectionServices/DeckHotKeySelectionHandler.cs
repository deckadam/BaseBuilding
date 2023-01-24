using System.Linq;
using Deck.Services.Implementations.MapService;
using Deck.UI.Hotkey.Events;
using Deck.Utility.Logger;
using UnityEngine;

namespace Deck.Services.Implementations.CellSelectionService
{
    public class DeckHotKeySelectionHandler
    {
        private DeckSelectionService _selectionService;
        private DeckMapService _mapService;

        public DeckHotKeySelectionHandler()
        {
            _mapService = Deck.GetService<DeckMapService>();
            _selectionService = Deck.GetService<DeckSelectionService>();
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
            var agents = Deck.GetService<DeckMapService>().GetAgents().ToArray();
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