using System.Collections.Generic;
using EventManager;
using General;
using InGame.Agent.Raid;
using Services.Raid.Events;
using Unity.Behavior;
using UnityEngine;

namespace InGame.Agent.Soldier.Skeleton
{
    public class DeckAgentSoldierSkeleton : DeckAgentSoldierBasic,IPatrollable
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
            // AddCommand(new DeckCommandSearchFor<DeckAgentRaider>(this, OnFound));
        }

        private void OnFound(DeckAgentRaider raider)
        {
            Debug.LogError("Agent found", raider.gameObject);
        }

        public void AddPatrolPoint(Vector3 point)
        {
            behaviourAgent.GetVariable("PatrolPoints",out BlackboardVariable<List<Vector3>> points);
            points.Value.Add(point);
        }
    }
}