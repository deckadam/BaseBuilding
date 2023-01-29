using Deck.Save;
using Deck.Services;
using Deck.Services.Implementations;
using Deck.GameManager.Constants;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu.Events;
using Deck.Utility.Logger;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.UI.SaveListingMenu
{
    public class DeckSaveDisplayer : MonoBehaviour, IPoolable<IMemoryPool>, IPointerClickHandler
    {
        public static DeckSaveDisplayer currentlySelectedDsiplayer => _currentlySelectedDisplayer;
        [ClearOnReload] private static DeckSaveDisplayer _currentlySelectedDisplayer;

        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private RectTransform rectTransform;
        private DeckSaveSystem.SaveFile _saveFile;
        private IMemoryPool _pool;

        public void SetData(DeckSaveSystem.SaveFile saveFile)
        {
            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void ReturnToPool()
        {
            _pool.Despawn(this);
        }

        public void OnDespawned()
        {
        }

        public void OnSpawned(IMemoryPool p1)
        {
            _pool = p1;
        }

        public class Factory : PlaceholderFactory<DeckSaveDisplayer>
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _currentlySelectedDisplayer = this;

            if (eventData.clickCount == 2)
            {
                var newConfirmationPopUp = Deck.GetService<DeckPopUpService>().GetPopUp<DeckConfirmationPopUp, DeckConfirmationPopUp.Factory>().Create();
                var parent = Deck.GetService<DeckUIService>().GetUI<DeckSaveListingMenu>().GetRectTransform();
                newConfirmationPopUp.transform.SetParent(parent);
                newConfirmationPopUp.transform.localPosition = Vector2.zero;
                newConfirmationPopUp.Initialize(DeckConfirmationDialogueConstants.loadSaveFileDialogue, () => OnLoadRequestedEvent.Create(_saveFile).Send(), null);
            }
        }

        public DeckSaveSystem.SaveFile GetSaveFile() => _saveFile;
    }
}