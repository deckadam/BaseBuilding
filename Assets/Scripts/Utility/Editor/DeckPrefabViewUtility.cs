using UnityEditor;
using UnityEditor.SceneManagement;

namespace Deck.Utility.Editor
{
    [InitializeOnLoad]
    public class DeckPrefabViewUtility
    {
        static DeckPrefabViewUtility()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabStageClosing += OnPrefabStageClosing;
        }

        private static bool wasPrefabOpened = false;

        static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            if (prefabStage == null)
                return;

            var root = prefabStage.prefabContentsRoot;
            wasPrefabOpened = root.activeSelf;
            root.gameObject.SetActive(true);
        }

        static void OnPrefabStageClosing(PrefabStage prefabStage)
        {
            if (prefabStage == null)
                return;

            var root = prefabStage.prefabContentsRoot;
            root.SetActive(wasPrefabOpened);
        }
    }
}