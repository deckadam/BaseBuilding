using Cysharp.Threading.Tasks;
using Deck.EventManager;
using Deck.GameManager;
using Deck.Save;
using Deck.SaveListingMenu.Events;
using Deck.Services;
using Deck.UI.MainMenu.Events;
using Deck.UI.Saves;
using Deck.Utility;
using UnityEngine;
using UnityEngine.UI;

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
            Deck.GetService<DeckGameManager>().LoadGame();
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

            Deck.GetService<DeckGameManager>().CreateNewGame();
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
            Deck.GetService<DeckGameManager>().LoadGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            _loadingStarted = false;

            DeckLogger.Inform("Last save file loaded");
        }

        public void OnLoadGame()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().Appear().Forget();
        }
    }
}