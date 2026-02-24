using Base;
using Commands;
using InGame.Agent.Raid;
using Services;
using Services.Raid;
using UnityEngine;

namespace InGame.AI.Raider
{
    public class DeckAIMoveTowardsChest : StateMachineBehaviour
    {
        private DeckAgentRaider _raider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _raider = animator.gameObject.GetComponent<DeckAgentRaider>();
            var chest = DeckServiceProvider.GetService<DeckServiceRaid>().GetMainChest();
            _raider.AddCommand(new DeckCommandMove(chest.GetPosition(),_raider).RegisterToOnCompleted(OnReachedToChest));
        }

        private void OnReachedToChest(DeckAgent obj)
        {
            DeckServiceProvider.GetService<DeckServiceRaid>().GetRaidController().OnRaiderReachedToChest(obj);
        }
    }
}