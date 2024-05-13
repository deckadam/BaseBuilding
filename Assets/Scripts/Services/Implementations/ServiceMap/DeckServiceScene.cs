using Cysharp.Threading.Tasks;
using Deck.Services;
using Deck.Utility.Logger;
using UnityEngine.SceneManagement;

namespace Deck.Services.MapService
{
    public class DeckServiceScene : DeckServiceBase
    {
        private static DeckMap sceneParent;

        public static DeckMap GetMap()
        {
            if (sceneParent != null)
            {
                return sceneParent;
            }

            sceneParent = FindObjectOfType<DeckMap>();
            return sceneParent;
        }

        public async UniTask LoadMap(bool isEmpty)
        {
            var mapName = isEmpty ? "MapScene_Empty" : "MapScene";
            var isLoaded = SceneManager.GetSceneByName(mapName).isLoaded;
            if (isLoaded)
            {
                DeckLogger.Map("Old map scene unloading");
                await SceneManager.UnloadSceneAsync(mapName);
                DeckLogger.Map("Old map scene unloaded");
            }

            DeckLogger.Map("New map scene loading");
            await SceneManager.LoadSceneAsync(mapName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(mapName));
            DeckLogger.Map("New map scene loaded");
        }
    }
}