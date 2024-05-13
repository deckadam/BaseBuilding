using System.Collections.Generic;
using Deck.UI;
using Zenject;

namespace Deck.Services
{
    public class DeckServicePopUp : DeckServiceBase
    {
        private Stack<DeckPopUpBase> _popUpStack = new();
        private DeckFactoryProviderUI _factoryProvider;

        [Inject]
        private void Inject(DeckFactoryProviderUI factoryProvider)
        {
            _factoryProvider = factoryProvider;
        }

        public T OpenPopUp<T, J>() where T : DeckPopUpBase where J : PlaceholderFactory<T>
        {
            var result = _factoryProvider.GetFactory<T, J>().Create();
            _popUpStack.Push(result);
            return result;
        }

        public void CloseLastPopUp()
        {
            if (_popUpStack.Count != 0)
            {
                var lastPopUp = _popUpStack.Pop();
                lastPopUp.OnCloseRequested();
                return;
            }

            Deck.GetService<DeckServiceUI>().GetUI<DeckMainMenu>().SwapAppearanceStatus();
        }
    }
}