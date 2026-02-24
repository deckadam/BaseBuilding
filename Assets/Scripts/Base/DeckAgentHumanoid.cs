using System;
using Commands;
using Components;
using Services;
using Services.Building.Buildable;
using Services.Building.Buildable.Data.Parameter.Implementations.Build;
using Services.ItemVisual;
using UnityEngine;
using Utility;

namespace Base
{
    public class DeckAgentHumanoid : DeckAgent
    {
        [SerializeField] private DeckBuildable buildingData;
        public Action<DeckAgentHumanoid> OnDeath;

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

        public void InitializeHumanoid()
        {
            if (!buildingData.TryGetParameter(out DeckBuildableParameterBuildModeGridBased parameterBuildModeGridBased))
            {
                DeckServiceProvider.GetService<DeckServiceItemVisual>().RequestItemVisual(this, buildingData.ItemVisual.PrefabId, out itemVisualInstance);
                RaiseItemVisualChanged();

                itemVisualInstance.transform.parent = SelfTransform;

                itemVisualInstance.transform.localPosition = Vector3.zero;
                itemVisualInstance.transform.localRotation = Quaternion.identity;
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
            instanceProvider.ReturnAgent(this);
            OnDeath?.Invoke(this);
        }

        protected sealed override void AfterInitializationCompleted()
        {
            InternalHumanoidSpawnRequested();
        }

        protected virtual void InternalHumanoidDespawnRequested()
        {
        }

        protected virtual void InternalHumanoidSpawnRequested()
        {
        }
    }
}