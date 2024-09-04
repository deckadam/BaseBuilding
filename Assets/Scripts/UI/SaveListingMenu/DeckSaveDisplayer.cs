using Deck.InGame.Agent.Building.Inventory;
using Deck.InGame.Agent.Building.SaveListingMenu.Events;
using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using Deck.Utility.Poolable;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Deck.InGame.Agent.Building.SaveListingMenu
{
    public class DeckSaveDisplayer : DeckUIElement, IPointerClickHandler
    {
        [field: ClearOnReload] public static DeckSaveDisplayer CurrentlySelectedDsiplayer { get; private set; }

        [SerializeField] private TextMeshProUGUI displayText;

        private DeckSaveSystem.SaveFile _saveFile;

        public void SetData(DeckSaveSystem.SaveFile saveFile)
        {

            _saveFile = saveFile;
            displayText.text = saveFile.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CurrentlySelectedDsiplayer = this;

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