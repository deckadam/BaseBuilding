using Cysharp.Threading.Tasks;
using Deck.Services;
using Deck.Test;
using Deck.Utility.Logger;

namespace Deck.UI.MainMenu
{
    public class DeckMainMenu : DeckUIBase
    {
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
        }

        public void OnLoadGame()
        {
        }
    }
}