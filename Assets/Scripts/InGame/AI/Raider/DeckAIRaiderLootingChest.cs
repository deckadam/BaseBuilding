using Commands;
using GameManager.Data.GameSetting;
using InGame.Agent.Raid;
using Services;
using Services.Map;
using Services.Raid;
using UnityEngine;

namespace InGame.AI.Raider
{
    public class DeckAIRaiderLootingChest : StateMachineBehaviour
    {
        private DeckServiceRaid _serviceRaid;
        private DeckAgentRaider _raider;
        private DeckGameSettingBasic _gameSetting;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _serviceRaid = DeckServiceProvider.GetService<DeckServiceRaid>();
            _raider = animator.GetComponent<DeckAgentRaider>();
            _gameSetting = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().GetGameSetting();
            _raider.AddCommand(new DeckCommandLootChest(_serviceRaid.GetMainChest(), _raider, _gameSetting.TotalLootDuration).RegisterToOnCompleted(OnCompleted));
        }

        private void OnCompleted()
        {
            _serviceRaid.GetRaidController().OnChestLooted();
        }
    }
}