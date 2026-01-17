using Cysharp.Threading.Tasks;
using InGame.Map;
using UnityEngine.SceneManagement;
using Utility;

namespace Services.Map
{
    public class DeckServiceScene : DeckServiceBase
    {
        private const string MapName = "MapScene";
        private static DeckMap _sceneParent;

        public static DeckMap GetMap()
        {
            if (_sceneParent != null)
            {
                return _sceneParent;
            }

            DeckLogger.Inform("Map not found looking at scene");
            _sceneParent = FindObjectOfType<DeckMap>();
            return _sceneParent;
        }

        public async UniTask LoadMap()
        {
            var isLoaded = SceneManager.GetSceneByName(MapName).isLoaded;
            if (isLoaded)
            {
                DeckLogger.Map("Map scene unloading");
                DeckServiceProvider.BeforeGameSceneUnloaded();
                await SceneManager.UnloadSceneAsync(MapName);
                DeckLogger.Map("Map scene unloaded");
            }

            DeckLogger.Map("New map scene loading");
            await SceneManager.LoadSceneAsync(MapName, LoadSceneMode.Additive);
            DeckServiceProvider.BeforeGameSessionInitialized();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(MapName));
            DeckServiceProvider.AfterGameSessionInitialized();
            DeckLogger.Map("New map scene loaded");
        }
    }
}