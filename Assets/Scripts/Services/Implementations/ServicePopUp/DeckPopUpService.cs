using Deck.Utility.Constants;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Deck.Services.Implementations
{
    public class DeckPopUpService : DeckServiceBase
    {
        [SerializeField, InfoBox("Object doesn't contain DeckPopUpBase implementing component", nameof(CheckValidity))] private GameObject[] popups;
        private DeckPopUpFactoryProvider _factoryProvider;

        [Inject]
        private void Inject(DeckPopUpFactoryProvider factoryProvider)
        {
            _factoryProvider = factoryProvider;
            _factoryProvider.Initialize();
        }

        private bool CheckValidity()
        {
            foreach (var popup in popups)
            {
                if (popup.GetComponent<DeckPopUpBase>() == null)
                {
                    return true;
                }
            }

            return false;
        }

        public J GetPopUp<T, J>()
        {
            return _factoryProvider.GetFactory<T, J>();
        }
    }
}