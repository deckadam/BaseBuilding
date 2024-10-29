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
        private DeckSaveListingMenu _saveListingMenu;
        public void SetData(DeckSaveSystem.SaveFile saveFile, DeckSaveListingMenu saveListingMenu)
        {
            _saveListingMenu = saveListingMenu;
            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _saveListingMenu.SetSelected(this);
            if (eventData.clickCount != 2) return;

            var newConfirmationPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = Deck.GetService<DeckServiceUI>().GetUI<DeckSaveListingMenu>().GetRectTransform();
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