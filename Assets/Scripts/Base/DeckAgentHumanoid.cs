using System;
using Commands;
using Components;
using Components.Movement;
using General;
using Services;
using Services.Building.Buildable;
using Services.ItemVisual;
using UnityEngine;

namespace Base
{
    public class DeckAgentHumanoid : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        public Action<DeckAgentHumanoid> OnDeath;

        public void InitializeHumanoid()
        {
            DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance);
            RaiseItemVisualChanged();

            itemVisualInstance.transform.parent = SelfTransform;

            itemVisualInstance.transform.localPosition = Vector3.zero;
            itemVisualInstance.transform.localRotation = Quaternion.identity;
        }

        public void AddCommand(DeckCommand command, bool isInterruptingCommand = false)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.AddCommand(command, isInterruptingCommand);
            }
        }

        public void EnqueueCommand(DeckCommand command)
        {
            if (TryGetDeckComponent<DeckComponentCommandProcessor>(out var commandProcessor))
            {
                commandProcessor.EnqueueCommand(command);
            }
        }

        public void OnWaiting()
        {
            InternalOnWaiting();
        }

        protected virtual void InternalOnWaiting()
        {
        }

        protected sealed override void InternalRequestDestroy()
        {
            InternalHumanoidDespawnRequested();
            OnDeath?.Invoke(this);
        }

        protected sealed override void AfterSpawned()
        {
            InternalHumanoidSpawnRequested();
        }

        protected virtual void InternalHumanoidDespawnRequested()
        {
        }

        protected virtual void InternalHumanoidSpawnRequested()
        {
        }

        public override void SetPosition(Vector3 position)
        {
            var movementComponent = GetDeckComponent<DeckComponentMovement>();
            movementComponent.Warp(position);
        }

        public override void SetPosition(IDeckTranslatable positioner)
        {
            var movementComponent = GetDeckComponent<DeckComponentMovement>();
            movementComponent.Warp(positioner.GetPosition());
        }
    }
}