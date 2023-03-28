using System.Collections.Generic;
using Deck;
using Deck.EventManager;
using Deck.InputHandling.Events;
using Deck.Utility.Logger;
using Deck.Events;
using Deck.UI.Hotkey.Events;
using UnityEngine;

namespace Deck.Events.CellSelectionService
{
    public class DeckHotKeySelectionHandler
    {
        private DeckServiceSelection _serviceSelection;
        private List<DeckAgentCore> _agents;

        public void Initialize()
        {
            _agents = new List<DeckAgentCore>();
            _serviceSelection = global::Deck.Deck.GetService<DeckServiceSelection>();
            DeckEventManager.Register<DeckOnAgentPossessedEvent>(OnAgentPossessed);
            DeckEventManager.Register<DeckOnCoreAgentCreatedEvent>(OnCoreAgentCreated);
            DeckEventManager.Register<DeckOnCoreAgentDeathEvent>(OnCoreAgentDeath);
        }

        private void OnCoreAgentDeath(DeckOnCoreAgentDeathEvent obj)
        {
            _agents.Remove(obj.agent);
            DeckOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        private void OnCoreAgentCreated(DeckOnCoreAgentCreatedEvent obj)
        {
            _agents.Add(obj.agent);
            DeckOnActiveHotkeyCountChanged.Create(_agents.Count).Send();
        }

        public void DeInitialize()
        {
            DeckEventManager.Unregister<DeckOnAgentPossessedEvent>(OnAgentPossessed);
        }

        private void OnAgentPossessed(DeckOnAgentPossessedEvent obj)
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

            DeckOnHotkeySelected.Create(index).Send();
            var agentToPossess = _agents[index];
            _serviceSelection.OnPossession(agentToPossess);
        }
    }
}