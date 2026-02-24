using InGame.Agent.Raid;
using Services;
using Services.Raid;
using UnityEngine;

namespace InGame.AI.Raider
{
    public class DeckAIRaiderInitialState : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var raidController = DeckServiceProvider.GetService<DeckServiceRaid>().GetRaidController();
            raidController.RegisterRaider(animator.gameObject.GetComponent<DeckAgentRaider>());
        }
    }
}