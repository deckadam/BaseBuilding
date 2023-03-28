using Deck.Services;
using Zenject;

namespace Deck.Events
{
    public class DeckServicePopUp : DeckServiceBase
    {
        private DeckFactoryProviderUI _factoryProvider;

        [Inject]
        private void Inject(DeckFactoryProviderUI factoryProvider)
        {
            _factoryProvider = factoryProvider;
        }

        public J GetPopUp<T, J>()
        {
            return _factoryProvider.GetFactory<T, J>();
        }
    }
}