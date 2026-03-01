using EventManager;
using GameManager.Data.GameSetting;
using GameManager.Events;
using InGame.Agent.Chest;
using InGame.Agent.Chest.Events;
using Instancing;
using Services.Raid.Events;
using Utility;
using Zenject;

namespace Services.Raid
{
    public class DeckServiceRaid : DeckServiceBase
    {
        private DeckGameSettingBasic _gameSetting;
        private DeckInstanceProvider _instanceProvider;
        private DeckBuildingChest _mainChest;
        private bool _isRaidOnProgress;

        private DeckAgentRaidController _currentRaidController;


        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public override void Initialize()
        {
            _isRaidOnProgress = false;
            DeckEventManager.Register<DeckEventOnRaidStartRequested>(OnRaidStartRequested);
            DeckEventManager.Register<DeckEventOnGameSettingsLoaded>(OnGameSettingsLoaded);
            DeckEventManager.Register<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Register<DeckEventOnMainChestDestroyed>(OnMainChestDestroyed);
        }

        protected override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnRaidStartRequested>(OnRaidStartRequested);
            DeckEventManager.Unregister<DeckEventOnGameSettingsLoaded>(OnGameSettingsLoaded);
            DeckEventManager.Unregister<DeckEventOnMainChestBuilt>(OnMainChestBuilt);
            DeckEventManager.Unregister<DeckEventOnMainChestDestroyed>(OnMainChestDestroyed);
        }

        private void OnMainChestBuilt(DeckEventOnMainChestBuilt obj)
        {
            _mainChest = obj.chest;
        }

        private void OnMainChestDestroyed(DeckEventOnMainChestDestroyed obj)
        {
            _mainChest = null;
        }

        private void OnGameSettingsLoaded(DeckEventOnGameSettingsLoaded obj)
        {
            _gameSetting = obj.gameSetting;
            _currentRaidController = _instanceProvider.RentAgent<DeckAgentRaidController>();
            _currentRaidController.Initialize(_gameSetting);
        }

        private void OnRaidStartRequested(DeckEventOnRaidStartRequested obj)
        {
            if (_isRaidOnProgress)
            {
                return;
            }

            _isRaidOnProgress = true;
            _currentRaidController.GenerateRaid(obj.mainChest);
        }

        internal void OnRaidEnded(bool result)
        {
            if (!_isRaidOnProgress)
            {
                DeckLogger.Error("Raid ended when not in progress state");
                return;
            }

            _isRaidOnProgress = false;

            if (result)
            {
                DeckLogger.Error("Raid ended with success");
            }
            else
            {
                DeckLogger.Error("Raid ended with fail");
            }

            DeckEventOnRaidEnded.Create().Send();
        }

        public DeckAgentRaidController GetRaidController()
        {
            return _currentRaidController;
        }

        public DeckBuildingChest GetMainChest()
        {
            return _mainChest;
        }

        public void SetForceRaidStartedStatus()
        {
            _isRaidOnProgress = true;
        }
    }
}