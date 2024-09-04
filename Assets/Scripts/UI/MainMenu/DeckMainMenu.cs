using Cysharp.Threading.Tasks;
using Deck.EventManager;
using Deck.InGame.Agent.Building.SaveListingMenu;
using Deck.InGame.Agent.Building.SaveListingMenu.Events;
using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 4014

namespace Deck.InGame.Agent.Building
{
    public class DeckMainMenu : DeckUIBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadLastSaveButton;
        [SerializeField] private Button loadSavesButton;

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
            Disappear();
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
            DeckLogger.Inform("New game starting");

            Deck.GetService<DeckGameManager>().CreateNewGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            DeckLogger.Inform("New game started");
        }

        public async void OnLastSaveLoad()
        {
            DeckSaveSystem.LoadLastSaveData();
            Deck.GetService<DeckGameManager>().LoadGame();
            DeckEventOnGameSceneLoaded.Create().Send();

            await UniTask.NextFrame();
            await Disappear();

            DeckLogger.Inform("Last save file loaded");
        }

        public void OnLoadGame()
        {
            Deck.GetService<DeckServiceUI>().GetUI<DeckSaveListingMenu>().Appear();
        }
    }
}