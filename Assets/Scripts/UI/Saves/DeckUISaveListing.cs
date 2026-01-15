using System.Collections.Generic;
using Deck.SaveListingMenu.Events;
using Deck.Services.PopUp;
using Deck.Services.UI;
using Deck.UI.Confirmation;
using Deck.Utility.Constants;
using Services;
using Systems.SystemSave;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Saves
{
    public class DeckUISaveListing : DeckUIBase
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private List<DeckSaveDisplayer> _activeDisplays = new();
        private DeckSaveDisplayer _lastSelectedDisplay;

        protected override void OnPreAppear()
        {
            _lastSelectedDisplay = null;
            InitializeSaveDisplays();
        }

        private void InitializeSaveDisplays()
        {
            if (_activeDisplays != null)
            {
                InstanceProvider.ReturnUIElement(_activeDisplays);
            }

            _activeDisplays.Clear();


            var saveFiles = DeckSaveSystem.GetAllSaves();
            AdjustScrollWindowSize(saveFiles.Length);

            foreach (var saveFile in saveFiles)
            {
                var newDisplay = InstanceProvider.RentUIElement<DeckSaveDisplayer>();
                newDisplay.SetData(saveFile, this);
                newDisplay.transform.SetParent(scrollParent, false);
                newDisplay.transform.localPosition = Vector2.zero;
                _activeDisplays.Add(newDisplay);
            }
        }

        private void AdjustScrollWindowSize(int count)
        {
            var result = 0f;
            var topGap = gridLayoutGroup.padding.top;
            result += topGap;
            result += (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y) * count;

            scrollParent.sizeDelta = new Vector2(scrollParent.sizeDelta.x, result);
        }

        public void OnLoadButtonClicked()
        {
            if (_lastSelectedDisplay == null)
            {
                return;
            }

            DeckEventOnLoadRequested.Create(_lastSelectedDisplay.GetSaveFile()).Send();
        }

        public void OnDeleteButtonClicked()
        {
            if (_lastSelectedDisplay == null)
            {
                return;
            }

            var newConfirmationPopUp = DeckServiceProvider.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = DeckServiceProvider.GetService<DeckServiceUI>().GetUI<DeckUISaveListing>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConstantsConfirmationDialogue.deleteSaveFileDialogue, () =>
            {
                DeckSaveSystem.DeleteSaveFile(_lastSelectedDisplay.GetSaveFile());
                InitializeSaveDisplays();
            }, null);
        }

        public void SetSelected(DeckSaveDisplayer saveDisplayer)
        {
            _lastSelectedDisplay = saveDisplayer;
        }
    }
}