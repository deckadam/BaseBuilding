using UnityEditor;
using UnityEditor.SceneManagement;

namespace Deck.Utility.Editor
{
    public class DeckSceneSwapHotkeys
    {
        [MenuItem("Deck/Open main scene %M")]
        private static void OpenMainScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");
        }

        [MenuItem("Deck/Open game scene %G")]
        private static void OpenMapScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/MapScene.unity");
        }
    }
}