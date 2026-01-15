using Cysharp.Threading.Tasks;
using Deck.Utility;
using InGame.Map;
using Services;
using UnityEngine.SceneManagement;
using Utility;

namespace Deck.Services.Map
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

            _sceneParent = FindObjectOfType<DeckMap>();
            return _sceneParent;
        }

        public async UniTask LoadMap()
        {
            var isLoaded = SceneManager.GetSceneByName(MapName).isLoaded;
            if (isLoaded)
            {
                DeckLogger.Map("Map scene unloading");
                global::Services.DeckServiceProvider.BeforeGameSceneUnloaded();
                await SceneManager.UnloadSceneAsync(MapName);
                DeckLogger.Map("Map scene unloaded");
            }

            DeckLogger.Map("New map scene loading");
            await SceneManager.LoadSceneAsync(MapName, LoadSceneMode.Additive);
            global::Services.DeckServiceProvider.BeforeGameSessionInitialized();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(MapName));
            global::Services.DeckServiceProvider.AfterGameSessionInitialized();
            DeckLogger.Map("New map scene loaded");
        }
    }
}