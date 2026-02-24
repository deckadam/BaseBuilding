using Base;
using Commands;
using InGame.Agent.Raid;
using Services;
using Services.Map;
using UnityEngine;
using Utility;

namespace InGame.AI.Raider
{
    public class DeckAIRaiderRunAway : StateMachineBehaviour
    {
        private DeckAgentRaider _raider;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var startingPoint = DeckServiceProvider.GetService<DeckServiceSession>().GetCurrentSession().RaidStartPoint;
            _raider = animator.GetComponent<DeckAgentRaider>();
            _raider.AddCommand(new DeckCommandMove(startingPoint.GetPosition(), _raider).RegisterToOnCompleted(OnRunAwayCompleted));
        }

        private void OnRunAwayCompleted(DeckAgent obj)
        {
            var raider = obj as DeckAgentRaider;
            if (raider == null)
            {
                DeckLogger.Error("Wrong cast", obj.gameObject);
                return;
            }

            raider.OnRunAwayFinished();
        }
    }
}