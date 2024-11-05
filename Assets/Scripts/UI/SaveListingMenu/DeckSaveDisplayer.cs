using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using Deck.Components.Building.Inventory;
using Deck.Components.Building.SaveListingMenu.Events;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck.Components.Building.SaveListingMenu
{
    public class DeckSaveDisplayer : DeckUIElement, IPointerClickHandler
    {

        [SerializeField] private TextMeshProUGUI displayText;

        private DeckSaveSystem.SaveFile _saveFile;
        private DeckUISaveListing _uiSaveListing;
        public void SetData(DeckSaveSystem.SaveFile saveFile, DeckUISaveListing uiSaveListing)
        {
            _uiSaveListing = uiSaveListing;
            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _uiSaveListing.SetSelected(this);
            if (eventData.clickCount != 2) return;

            var newConfirmationPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = Deck.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConstantsConfirmationDialogue.loadSaveFileDialogue, () => DeckEventOnLoadRequested.Create(_saveFile).Send(), null);
        }

        public DeckSaveSystem.SaveFile GetSaveFile()
        {
            return _saveFile;
        }
    }
}