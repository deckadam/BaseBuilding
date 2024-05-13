using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.UI.SaveListingMenu
{
    public class DeckSaveDisplayer : MonoBehaviour, IPoolable<IMemoryPool>, IPointerClickHandler
    {
        [field: ClearOnReload] public static DeckSaveDisplayer CurrentlySelectedDsiplayer { get; private set; }

        [SerializeField] private TextMeshProUGUI displayText;
        [SerializeField] private RectTransform rectTransform;

        private DeckSaveSystem.SaveFile _saveFile;
        private IMemoryPool _pool;

        public void SetData(DeckSaveSystem.SaveFile saveFile)
        {
            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CurrentlySelectedDsiplayer = this;

            if (eventData.clickCount != 2) return;
            
            var newConfirmationPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp, DeckConfirmationPopUp.Factory>();
            var parent = Deck.GetService<DeckServiceUI>().GetUI<DeckSaveListingMenu>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConstantsConfirmationDialogue.loadSaveFileDialogue, () => OnLoadRequestedEvent.Create(_saveFile).Send(), null);
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

        public DeckSaveSystem.SaveFile GetSaveFile()
        {
            return _saveFile;
        }

        public class Factory : PlaceholderFactory<DeckSaveDisplayer>
        {
        }
    }
}