using Commands;
using Components;

namespace Base
{
    public class DeckAgentHumanoid : DeckAgent
    {
        public void AddCommand(DeckCommand command, bool isInterruptingCommand)
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
            instanceProvider.ReturnAgent(this);
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