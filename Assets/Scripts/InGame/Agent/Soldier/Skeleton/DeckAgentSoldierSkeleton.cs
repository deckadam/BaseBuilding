using Commands;
using EventManager;
using InGame.Agent.Raid;
using Services.Raid.Events;
using UnityEngine;

namespace InGame.Agent.Soldier.Skeleton
{
    public class DeckAgentSoldierSkeleton : DeckAgentSoldierBasic
    {
        protected override void InternalHumanoidSpawnRequested()
        {
            DeckEventManager.Register<DeckEventOnRaidStarted>(OnRaidStarted);
        }

        protected override void InternalHumanoidDespawnRequested()
        {
            DeckEventManager.Unregister<DeckEventOnRaidStarted>(OnRaidStarted);
        }

        private void OnRaidStarted(DeckEventOnRaidStarted obj)
        {
            AddCommand(new DeckCommandSearchFor<DeckAgentRaider>(this, OnFound));
        }

        private void OnFound(DeckAgentRaider raider)
        {
            Debug.LogError("Agent found", raider.gameObject);
        }
    }
}