using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;
using Deck.General;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Utility.Constants;

namespace Deck.Commands.Sit
{
    public class DeckCommandSit : DeckCommand
    {
        private DeckAgent _agent;
        private DeckAgent _target;

        public DeckCommandSit()
        {
        }

        public DeckCommandSit(DeckAgent agent, DeckAgent target)
        {
            _agent = agent;
            _target = target;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_target.transform.position, _agent).ProcessCommand(token);

            _agent.GetDeckComponent<DeckComponentMovement>().SetRotation(_target.transform.rotation);
            _agent.transform.rotation = _target.transform.rotation;

            var animatorHumanoid = _agent.GetDeckComponent<IDeckAnimationSetTrigger>();
            animatorHumanoid.Trigger(DeckConstantsAnimation.Sit);

            _target.GetComponent<IDeckSittable>().OnSit((DeckAgentHumanoid)_agent);

            await UniTask.Delay(1000, cancellationToken: token);
            return true;
        }

        public override string GetSaveData()
        {
            var data = new DeckCommandSitSaveData()
            {
                agentId = _agent.UniqueId.ID,
                targetId = _target.UniqueId.ID,
            };

            return DeckSaveUtility.GetSerializedData(data);
        }

        public override void LoadSaveData(string data)
        {
            var deserializedData = DeckSaveUtility.GetDeserializedData<DeckCommandSitSaveData>(data);
            _agent = Deck.GetService<DeckServiceFinder>().GetAgent(deserializedData.agentId);
            _target = Deck.GetService<DeckServiceFinder>().GetAgent(deserializedData.targetId);
        }

        [Serializable]
        private class DeckCommandSitSaveData
        {
            public int agentId;
            public int targetId;
        }
    }
}