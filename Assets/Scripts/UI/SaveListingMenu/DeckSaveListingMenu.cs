using System.Collections.Generic;
using Deck.Save;
using Deck.Services;
using Deck.Utility.Logger;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Deck.UI.SaveListingMenu
{
    public class DeckSaveListingMenu : DeckUIBase
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private List<DeckSaveDisplayer> _activeDisplayers = new();


        protected override void OnPreAppear()
        {
            InitializeSaveDisplayers();
        }

        private void InitializeSaveDisplayers()
        {
            if (_activeDisplayers != null)
            {
                uiPool.Return(_activeDisplayers);
            }

            _activeDisplayers.Clear();


            var saveFiles = DeckSaveSystem.GetAllSaves();
            AdjustScrollWindowSize(saveFiles.Length);

            for (var index = 0; index < saveFiles.Length; index++)
            {
                var newDisplayer = uiPool.Rent<DeckSaveDisplayer>();
                newDisplayer.SetData(saveFiles[index]);
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
            if (DeckSaveDisplayer.CurrentlySelectedDsiplayer == null)
            {
                return;
            }

            OnLoadRequestedEvent.Create(DeckSaveDisplayer.CurrentlySelectedDsiplayer.GetSaveFile()).Send();
        }

        public void OnDeleteButtonClicked()
        {
            if (DeckSaveDisplayer.CurrentlySelectedDsiplayer == null)
            {
                return;
            }

            var newConfirmationPopUp = Deck.GetService<DeckServicePopUp>().OpenPopUp<DeckConfirmationPopUp>();
            var parent = Deck.GetService<DeckServiceUI>().GetUI<DeckSaveListingMenu>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConstantsConfirmationDialogue.deleteSaveFileDialogue, () =>
            {
                DeckSaveSystem.DeleteSaveFile(DeckSaveDisplayer.CurrentlySelectedDsiplayer.GetSaveFile());
                InitializeSaveDisplayers();
            }, null);
        }
    }
}