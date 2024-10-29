using System.Collections.Generic;
using Deck.Components.Building.Inventory;
using Deck.Components.Building.SaveListingMenu.Events;
using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.Components.Building.SaveListingMenu
{
    public class DeckSaveListingMenu : DeckUIBase
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private List<DeckSaveDisplayer> _activeDisplayers = new();
        private DeckSaveDisplayer _lastSelectedDisplayer;

        protected override void OnPreAppear()
        {
            _lastSelectedDisplayer = null;
            InitializeSaveDisplayers();
        }

        private void InitializeSaveDisplayers()
        {
            if (_activeDisplayers != null)
            {
                InstanceProvider.ReturnUIElement(_activeDisplayers);
            }

            _activeDisplayers.Clear();


            var saveFiles = DeckSaveSystem.GetAllSaves();
            AdjustScrollWindowSize(saveFiles.Length);

            for (var index = 0; index < saveFiles.Length; index++)
            {
                var newDisplayer = InstanceProvider.RentUIElement<DeckSaveDisplayer>();
                newDisplayer.SetData(saveFiles[index],this);
                newDisplayer.transform.SetParent(scrollParent, false);
                newDisplayer.transform.localPosition = Vector2.zero;
                _activeDisplayers.Add(newDisplayer);
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
            if (_lastSelectedDisplayer == null)
            {
                return;
            }

            DeckEventOnLoadRequested.Create(_lastSelectedDisplayer.GetSaveFile()).Send();
        }

        public void OnDeleteButtonClicked()
        {
            if (_lastSelectedDisplayer == null)
            {
                return;
            }

            var newConfirmationPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = Deck.GetService<DeckServiceUI>().GetUI<DeckSaveListingMenu>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConstantsConfirmationDialogue.deleteSaveFileDialogue, () =>
            {
                DeckSaveSystem.DeleteSaveFile(_lastSelectedDisplayer.GetSaveFile());
                InitializeSaveDisplayers();
            }, null);
        }

        public void SetSelected(DeckSaveDisplayer saveDisplayer)
        {
            _lastSelectedDisplayer = saveDisplayer;
        }
    }
}