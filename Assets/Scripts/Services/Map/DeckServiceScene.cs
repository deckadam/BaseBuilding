using Cysharp.Threading.Tasks;
using Deck.Utility;
using UnityEngine.SceneManagement;

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
                Deck.BeforeGameSceneUnloaded();
                await SceneManager.UnloadSceneAsync(MapName);
                DeckLogger.Map("Map scene unloaded");
            }

            DeckLogger.Map("New map scene loading");
            await SceneManager.LoadSceneAsync(MapName, LoadSceneMode.Additive);
            Deck.BeforeGameSessionInitialized();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(MapName));
            Deck.AfterGameSessionInitialized();
            DeckLogger.Map("New map scene loaded");
        }
    }
}