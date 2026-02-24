using Base;
using Services;
using Services.Raid;
using UnityEngine;

namespace InGame.Agent.Raid
{
    public class DeckAgentRaider : DeckAgentHumanoid
    {
        private static readonly int StartAI = Animator.StringToHash("Start");
        private static readonly int LootingChest = Animator.StringToHash("LootChest");
        private static readonly int GuardingLooter = Animator.StringToHash("GuardLooter");
        private static readonly int RunAway = Animator.StringToHash("RunAway");

        [SerializeField] private Animator aiController;

        public void EnterStateInitial()
        {
            aiController.SetTrigger(StartAI);
        }

        public void EnterStateLootingChest()
        {
            aiController.Play(LootingChest);
        }

        public void EnterStateGuardingLooter()
        {
            aiController.Play(GuardingLooter);
        }

        public void EnterStateRunAway()
        {
            aiController.Play(RunAway);
        }

        public void OnRunAwayFinished()
        {
            DeckServiceProvider.GetService<DeckServiceRaid>().GetRaidController().OnRaiderRanAway(this);
            RequestDestroy();
        }
    }
}