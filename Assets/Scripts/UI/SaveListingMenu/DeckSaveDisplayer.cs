using Deck.SaveListingMenu.Events;
using Deck.Components;
using Deck.Components.Building;
using Deck.Utility.Logger;
using Deck.Components.Building.Inventory;
using Deck.Save;
using Deck.Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Deck.SaveListingMenu
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

            var newConfirmationPopUp = global::Deck.Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = global::Deck.Deck.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().GetRectTransform();
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