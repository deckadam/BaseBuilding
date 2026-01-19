using Cysharp.Threading.Tasks;
using EventManager;
using GameManager;
using GameManager.Data.GameSetting;
using GameManager.Events;
using Services;
using Services.UI;
using Systems.SystemSave;
using Systems.SystemSave.Events;
using UI.MainMenu.Events;
using UI.Saves;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.MainMenu
{
    public class DeckUIMainMenu : DeckUIBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadLastSaveButton;
        [SerializeField] private Button loadSavesButton;

        private bool _loadingInProgress;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventOnLoadRequested>(OnLoadRequested);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnLoadRequested>(OnLoadRequested);
        }

        private void OnLoadRequested(DeckEventOnLoadRequested obj)
        {
            DeckSaveSystem.LoadFromPath(obj.saveFile.path);
            DeckServiceProvider.GetService<DeckGameManager>().LoadGame();
            Disappear().Forget();
        }

        protected override void OnPreAppear()
        {
            var result = DeckSaveSystem.HasValidSaveFile();
            loadLastSaveButton.interactable = result;
            loadLastSaveButton.interactable = result;
            DeckEventOnMainMenuAppeared.Create().Send();
        }

        protected override void OnPostDisappear()
        {
            DeckEventOnMainMenuDisappear.Create().Send();
        }

        public async void OnNewGame(DeckGameSettingBasic gameSettingBasic)
        {
            if (_loadingInProgress)
            {
                return;
            }

            _loadingInProgress = true;
            DeckLogger.Inform("New game starting");

            await DeckServiceProvider.GetService<DeckGameManager>().CreateNewGame();
            
            DeckEventOnGameSceneLoaded.Create().Send();
            DeckEventOnGameSettingsLoaded.Create(gameSettingBasic).Send();
            
            gameSettingBasic.OnSettingLoadedNewGame();

            await UniTask.NextFrame();
            await Disappear();

            _loadingInProgress = false;

            DeckLogger.Inform("New game started");
        }

        public async void OnLastSaveLoad()
        {
            if (_loadingInProgress)
            {
                return;
            }

            _loadingInProgress = true;

            DeckSaveSystem.LoadLastSaveData();
            DeckServiceProvider.GetService<DeckGameManager>().LoadGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            _loadingInProgress = false;

            DeckLogger.Inform("Last save file loaded");
        }

        public void OnLoadGame()
        {
            DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().Appear().Forget();
        }
    }
}