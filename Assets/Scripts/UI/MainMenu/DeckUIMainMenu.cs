using Cysharp.Threading.Tasks;
using Deck.SaveListingMenu.Events;
using Deck.Services.UI;
using EventManager;
using GameManager;
using Services;
using Systems.SystemSave;
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

        public async void OnNewGame()
        {
            if (_loadingInProgress)
            {
                return;
            }

            _loadingInProgress = true;
            DeckLogger.Inform("New game starting");

            DeckServiceProvider.GetService<DeckGameManager>().CreateNewGame();
            DeckEventOnGameSceneLoaded.Create().Send();

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