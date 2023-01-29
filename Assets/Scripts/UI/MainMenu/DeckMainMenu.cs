using Cysharp.Threading.Tasks;
using Deck.EventManager;
using Deck.Save;
using Deck.Services.Implementations;
using Deck.GameManager;
using Deck.UI.SaveListingMenu;
using Deck.UI.SaveListingMenu.Events;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 4014

namespace Deck.UI.MainMenu
{
    public class DeckMainMenu : DeckUIBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadLastSaveButton;
        [SerializeField] private Button loadSavesButton;


        public override void Initialize()
        {
            DeckEventManager.Register<OnLoadRequestedEvent>(OnLoadRequested);
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<OnLoadRequestedEvent>(OnLoadRequested);
        }

        private void OnLoadRequested(OnLoadRequestedEvent obj)
        {
            DeckSaveSystem.LoadFromPath(obj.saveFile.path);
            Deck.GetService<DeckGameManager>().LoadGame();
            Disappear();
        }

        public override void OnPreAppear()
        {
            var result = DeckSaveSystem.HasValidSaveFile();
            loadLastSaveButton.interactable = result;
            loadLastSaveButton.interactable = result;
        }

        public async void OnNewGame()
        {
            DeckLogger.Inform("New game starting");

            Deck.GetService<DeckGameManager>().CreateNewGame();

            await UniTask.NextFrame();
            await Disappear();

            DeckLogger.Inform("New game started");
        }

        public async void OnLastSaveLoad()
        {
            DeckSaveSystem.LoadLastSaveData();
            Deck.GetService<DeckGameManager>().LoadGame();

            await UniTask.NextFrame();
            await Disappear();

            DeckLogger.Inform("Last save file loaded");
        }

        public void OnLoadGame()
        {
            Deck.GetService<DeckUIService>().GetUI<DeckSaveListingMenu>().Appear();
        }
    }
}