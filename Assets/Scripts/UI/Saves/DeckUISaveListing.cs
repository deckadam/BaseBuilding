using System.Collections.Generic;
using Services;
using Services.Escapable;
using Services.PopUp;
using Services.UI;
using Systems.SystemSave;
using Systems.SystemSave.Events;
using UI.Confirmation;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Utility.Constants;

namespace UI.Saves
{
    public class DeckUISaveListing : DeckUIBase, IDeckEscapable
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        private List<DeckSaveDisplay> _activeDisplays = new();
        private DeckSaveDisplay _lastSelectedDisplay;

        protected override void OnPreAppear()
        {
            _lastSelectedDisplay = null;
            InitializeSaveDisplays();
            DeckServiceProvider.GetService<DeckServiceEscapable>().RegisterEscapable(this);
            HasEscaped = false;
        }

        private void InitializeSaveDisplays()
        {
            if (!_activeDisplays.IsNullOrEmpty())
            {
                InstanceProvider.ReturnUIElement(_activeDisplays);
            }

            _activeDisplays.Clear();


            var saveFiles = DeckSaveSystem.GetAllSaves();
            AdjustScrollWindowSize(saveFiles.Length);

            foreach (var saveFile in saveFiles)
            {
                var newDisplay = InstanceProvider.RentUIElement<DeckSaveDisplay>();
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

        public void SetSelected(DeckSaveDisplay saveDisplay)
        {
            _lastSelectedDisplay = saveDisplay;
        }
        
        public override void OnEscapeRequested()
        {
            base.OnEscapeRequested();
            HasEscaped = true;
        }

        public bool CanBeEscapedWithRightClick => false;
        public bool HasEscaped { get; private set; }
    }
}