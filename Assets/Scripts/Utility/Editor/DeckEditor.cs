using System.Collections.Generic;
using Data.Buildable;
using EventManager;
using Instancing;
using Systems.SystemInput.Events;
using UnityEditor;
using UnityEngine;

namespace Utility.Editor
{
    public static class DeckEditor
    {
        [MenuItem("Deck/Clear Listeners")]
        private static void ClearListeners()
        {
            DeckEventManager.ClearEvents<DeckEventMiddleScroll>();
            DeckEventManager.ClearEvents<DeckEventOnLeftClickDown>();
            DeckEventManager.ClearEvents<DeckEventOnLeftClickUp>();
            DeckEventManager.ClearEvents<DeckEventOnMouseMove>();
            DeckEventManager.ClearEvents<DeckEventOnMiddleMouseButtonStatusChange>();
        }


        [MenuItem("Deck/Free resources")]
        private static void FreeResources()
        {
            EditorUtility.UnloadUnusedAssetsImmediate();
            Resources.UnloadUnusedAssets();
        }

        [MenuItem("Deck/Check Uniqueness")]
        private static void CheckUniqueness()
        {
            var instanceProvider = Resources.FindObjectsOfTypeAll<DeckInstanceProvider>()[0];

            var agentHashSet = new HashSet<int>();
            foreach (var agent in instanceProvider.GetAgents())
            {
                if (!agentHashSet.Add(agent.PrefabId.Id))
                {
                    DeckLogger.Error("Multiple id " + agent.name);
                }
            }

            var itemVisualHashSet = new HashSet<int>();
            foreach (var itemVisual in instanceProvider.GetItemVisuals())
            {
                if (!itemVisualHashSet.Add(itemVisual.PrefabId.Id))
                {
                    DeckLogger.Error("Multiple id " + itemVisual.name);
                }
            }

            var uiElements = new HashSet<int>();
            foreach (var uiElement in instanceProvider.GetUIElements())
            {
                if (!uiElements.Add(uiElement.PrefabId.Id))
                {
                    DeckLogger.Error("Multiple id " + uiElement.name);
                }
            }

            var buildables = Resources.FindObjectsOfTypeAll<DeckBuildable>();
            var buildableIds = new HashSet<int>();
            foreach (var deckBuildable in buildables)
            {
                if (!buildableIds.Add(deckBuildable.Agent.PrefabId.Id))
                {
                    DeckLogger.Error("Multiple id " + deckBuildable.name, deckBuildable);
                }
            }
        }
    }
}