using Cysharp.Threading.Tasks;
using Services;
using Services.Map;
using Services.UI;
using UI.MainMenu;

namespace UI.InGame
{
    public class DeckUIInGame : DeckUIBase, IDeckEscapable
    {
        public bool HasEscaped { get; private set; }

        protected override void OnPreAppear()
        {
            HasEscaped = false;
        }

        public override void OnEscapeRequested()
        {
            Disappear().Forget();
            HasEscaped = true;
        }

        public void ResumeGameButton()
        {
            Disappear().Forget();
            HasEscaped = true;
        }

        public async void ReturnToMainMenuButton()
        {
            await DeckServiceProvider.GetService<DeckServiceSession>().UnloadCurrentSession();
            await DeckServiceProvider.GetService<DeckServiceUI>().ShowWindow<DeckUIMainMenu>();
            Disappear().Forget();
            HasEscaped = true;
        }
    }
}