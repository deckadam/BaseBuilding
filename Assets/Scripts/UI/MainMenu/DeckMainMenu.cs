using Cysharp.Threading.Tasks;
using Deck.Services;
using Deck.Services.Implementations.SaveService;
using Deck.Test;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.MainMenu
{
    public class DeckMainMenu : DeckUIBase
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadLastSaveButton;
        [SerializeField] private Button loadSavesButton;

        public override void OnPreAppear()
        {
            loadLastSaveButton.interactable = DeckSaveManager.HasValidSaveFile();
        }

        public async void OnNewGame()
        {
            DeckLogger.Inform("New game starting");

            DeckServiceLocator.GetService<DeckGameManager>().CreateNewGame();

            await UniTask.NextFrame();
            await Disappear();

            DeckLogger.Inform("New game started");
        }

        public void OnLastSaveLoad()
        {
            DeckSaveManager.LoadLastSaveData();
            Debug.LogError("Last save file loaded");
        }

        public void OnLoadGame()
        {
        }
    }
}