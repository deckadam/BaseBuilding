using Deck.Base;
using Deck.SaveListingMenu.Events;
using Deck.Services.PopUp;
using Deck.Services.UI;
using Deck.UI.Confirmation;
using Deck.Utility.Constants;
using Services;
using Systems.SystemSave;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace UI.Saves
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

            var newConfirmationPopUp = DeckServiceProvider.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().GetRectTransform();
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