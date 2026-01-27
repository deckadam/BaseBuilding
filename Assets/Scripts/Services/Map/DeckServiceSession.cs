using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameManager.Data.GameSetting;
using InGame.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace Services.Map
{
    public class DeckServiceSession : DeckServiceBase
    {
        private const string SceneName = "MapScene";
        private static DeckSession _currentSession;

        private bool _isSessionActive;

        public static DeckSession GetSession()
        {
            if (_currentSession != null)
            {
                return _currentSession;
            }

            DeckLogger.Inform("Scene not found looking at scene");
            _currentSession = FindObjectOfType<DeckSession>();
            return _currentSession;
        }

        public async UniTask LoadSession(DeckGameSettingBasic gameSetting)
        {
            var isLoaded = SceneManager.GetSceneByName(SceneName).isLoaded;
            if (isLoaded)
            {
                await UnloadCurrentSession();
            }

            await LoadNewSession(gameSetting);
            DeckLogger.Map("New map scene session");

            _isSessionActive = true;
        }

        private async Task LoadNewSession(DeckGameSettingBasic gameSettingBasic)
        {
            await SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);
            GetSession().Initialize(gameSettingBasic);
            DeckServiceProvider.BeforeGameSessionInitialized();
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(SceneName));
            DeckServiceProvider.AfterGameSessionInitialized();
        }

        public async Task UnloadCurrentSession()
        {
            DeckLogger.Map("Map scene unloading");
            DeckServiceProvider.BeforeGameSceneUnloaded();
            await SceneManager.UnloadSceneAsync(SceneName);
            DeckLogger.Map("Map scene unloaded");
        }

        public bool IsSessionActive()
        {
            return _isSessionActive;
        }

        public DeckSession GetCurrentSession()
        {
            return _currentSession;
        }

        public void GetCameraConfiner()
        {
            throw new System.NotImplementedException();
        }
    }
}