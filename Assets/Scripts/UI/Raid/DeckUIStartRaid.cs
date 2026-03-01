using EventManager;
using InGame.Agent.Chest;
using InGame.Agent.Chest.Events;
using Services.Raid.Events;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI.Raid
{
    public class DeckUIStartRaid : DeckUIBase
    {
        [SerializeField] private Button startRaidButton;

        private DeckBuildingChest _currentMainChest;
        private bool _isRaidInProgress;
        private bool _hasRaidTarget;

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventOnRaidStarted>(OnRaidStarted);
            DeckEventManager.Register<DeckEventOnRaidEnded>(OnRaidEnded);
            DeckEventManager.Register<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Register<DeckEventOnMainChestDestroyed>(OnMainChestDestroyed);

            startRaidButton.interactable = false;
        }

        public override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnRaidStarted>(OnRaidStarted);
            DeckEventManager.Unregister<DeckEventOnRaidEnded>(OnRaidEnded);
            DeckEventManager.Unregister<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Unregister<DeckEventOnMainChestDestroyed>(OnMainChestDestroyed);
        }

        private void OnMainChestBuilt(DeckEventOnMainChestBuilt obj)
        {
            _currentMainChest = obj.chest;
            _hasRaidTarget = true;
            startRaidButton.interactable = true;
        }

        private void OnMainChestDestroyed(DeckEventOnMainChestDestroyed obj)
        {
            _hasRaidTarget = false;
            startRaidButton.interactable = false;
            _currentMainChest = null;
        }

        private void OnRaidStarted(DeckEventOnRaidStarted obj)
        {
            _isRaidInProgress = true;
            startRaidButton.interactable = false;
        }

        private void OnRaidEnded(DeckEventOnRaidEnded obj)
        {
            _isRaidInProgress = false;
            startRaidButton.interactable = _hasRaidTarget;
        }

        public override async void AfterGameSessionInitialized()
        {
            await Appear();
        }

        public override async void BeforeGameSessionInitialized()
        {
            await Disappear();
        }


        public void OnRaidStartClicked()
        {
            if (!_hasRaidTarget)
            {
                return;
            }

            if (_isRaidInProgress)
            {
                return;
            }

            DeckEventOnRaidStartRequested.Create(_currentMainChest).Send();
        }
    }
}