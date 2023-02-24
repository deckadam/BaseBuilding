using System.Collections.Generic;
using System.Linq;
using Deck.EventManager;
using Deck.Save;
using Deck.Services;
using Deck.Services.Implementations;
using Deck.GameManager;
using Deck.GameManager.Constants;
using Deck.UI.Inventory;
using Deck.UI.SaveListingMenu.Events;
using Deck.Utility.Logger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Deck.UI.SaveListingMenu
{
    public class DeckSaveListingMenu : DeckUIBase
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private DeckSaveDisplayer.Factory _saveDisplayerFactory;
        private List<DeckSaveDisplayer> _activeDisplayers = new();

        [Inject]
        private void Inject(DeckSaveDisplayer.Factory saveDisplayerFactory)
        {
            _saveDisplayerFactory = saveDisplayerFactory;
        }

        public override void OnPreAppear()
        {
            InitializeSaveDisplayers();
        }

        private void InitializeSaveDisplayers()
        {
            if (_activeDisplayers != null && _activeDisplayers.Count != 0)
            {
                _activeDisplayers.ForEach(item => item.ReturnToPool());
                _activeDisplayers.Clear();
            }


            var saveFiles = DeckSaveSystem.GetAllSaves();
            AdjustScrollWindowSize(saveFiles.Length);

            for (var index = 0; index < saveFiles.Length; index++)
            {
                var newDisplayer = _saveDisplayerFactory.Create();
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
            if (DeckSaveDisplayer.currentlySelectedDsiplayer == null)
            {
                return;
            }

            OnLoadRequestedEvent.Create(DeckSaveDisplayer.currentlySelectedDsiplayer.GetSaveFile()).Send();
        }

        public void OnDeleteButtonClicked()
        {
            if (DeckSaveDisplayer.currentlySelectedDsiplayer == null)
            {
                return;
            }

            var newConfirmationPopUp = Deck.GetService<DeckPopUpService>().GetPopUp<DeckConfirmationPopUp, DeckConfirmationPopUp.Factory>().Create();
            var parent = Deck.GetService<DeckUIService>().GetUI<DeckSaveListingMenu>().GetRectTransform();
            newConfirmationPopUp.transform.SetParent(parent, false);
            newConfirmationPopUp.transform.localPosition = Vector2.zero;
            newConfirmationPopUp.Initialize(DeckConfirmationDialogueConstants.deleteSaveFileDialogue, () =>
            {
                DeckSaveSystem.DeleteSaveFile(DeckSaveDisplayer.currentlySelectedDsiplayer.GetSaveFile());
                InitializeSaveDisplayers();
            }, null);
        }
    }
}