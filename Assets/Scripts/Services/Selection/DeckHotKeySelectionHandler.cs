using System.Collections.Generic;
using Base;
using Deck.Services.Selection;
using Deck.Services.Selection.Events;
using Deck.UI.Hotkey.Events;
using EventManager;
using Systems.SystemInput.Events;
using UnityEngine;
using Utility;

namespace Services.Selection
{
    public class DeckHotKeySelectionHandler
    {
        private DeckServiceSelection _serviceSelection;
        private List<DeckAgent> _agents = new();

        public void Initialize()
        {
            _serviceSelection = DeckServiceProvider.GetService<DeckServiceSelection>();
            DeckEventManager.Register<DeckEventOnAgentPossessed>(OnAgentPossessed);
            DeckEventManager.Register<DeckEventOnCoreAgentCreated>(OnCoreAgentCreated);
            DeckEventManager.Register<DeckEventOnCoreAgentDeath>(OnCoreAgentDeath);
        }

        private void OnCoreAgentDeath(DeckEventOnCoreAgentDeath obj)
        {
            _agents.Remove(obj.agent);
            DeckEventOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        private void OnCoreAgentCreated(DeckEventOnCoreAgentCreated obj)
        {
            _agents.Add(obj.agent);
            DeckEventOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        public void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnAgentPossessed>(OnAgentPossessed);
        }

        private void OnAgentPossessed(DeckEventOnAgentPossessed obj)
        {
            var index = _agents.IndexOf(obj.agent);
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
            if (_agents.Count <= index)
            {
                return;
            }

            DeckEventOnHotkeySelected.Create(index).Send();
            var agentToPossess = _agents[index];
            _serviceSelection.OnPossession(agentToPossess);
        }
    }
}