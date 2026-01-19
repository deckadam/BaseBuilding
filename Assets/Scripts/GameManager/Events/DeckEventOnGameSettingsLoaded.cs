using EventManager;
using GameManager.Data.GameSetting;

namespace GameManager.Events
{
    public struct DeckEventOnGameSettingsLoaded : IDeckEvent
    {
        public DeckGameSettingBasic gameSetting { get; private set; }

        public static DeckEventOnGameSettingsLoaded Create(DeckGameSettingBasic mapData)
        {
            return new DeckEventOnGameSettingsLoaded
            {
                gameSetting = mapData
            };
        }
    }
}