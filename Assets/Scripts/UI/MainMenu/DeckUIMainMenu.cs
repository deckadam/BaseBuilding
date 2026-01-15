using Cysharp.Threading.Tasks;
using Deck.GameManager;
using Deck.SaveListingMenu.Events;
using Deck.Services.UI;
using Deck.UI.MainMenu.Events;
using Deck.UI.Saves;
using Deck.Utility;
using EventManager;
using Systems.SystemSave;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Deck.UI.MainMenu
{
    public class DeckUIMainMenu : DeckUIBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadLastSaveButton;
        [SerializeField] private Button loadSavesButton;

        private bool _loadingStarted;

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
            global::Services.DeckServiceProvider.GetService<DeckGameManager>().LoadGame();
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
            // if (_loadingStarted)
            // {
            //     return;
            // }

            _loadingStarted = true;
            DeckLogger.Inform("New game starting");

            global::Services.DeckServiceProvider.GetService<DeckGameManager>().CreateNewGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            _loadingStarted = false;

            DeckLogger.Inform("New game started");
        }

        public async void OnLastSaveLoad()
        {
            if (_loadingStarted)
            {
                return;
            }

            _loadingStarted = true;

            DeckSaveSystem.LoadLastSaveData();
            global::Services.DeckServiceProvider.GetService<DeckGameManager>().LoadGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            _loadingStarted = false;

            DeckLogger.Inform("Last save file loaded");
        }

        public void OnLoadGame()
        {
            global::Services.DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().Appear().Forget();
        }
    }
}