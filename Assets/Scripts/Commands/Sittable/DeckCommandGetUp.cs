using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Components;
using Deck.Save;
using Deck.Services.Finder;
using Deck.Utility.Constants;
using UnityEngine;

namespace Deck.Commands.Sittable
{
    public class DeckCommandGetUp : DeckCommand
    {
        private DeckAgentHumanoid _target;

        public DeckCommandGetUp()
        {
        }

        public DeckCommandGetUp(DeckAgentHumanoid target)
        {
            _target = target;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            Debug.LogError("Getting up");
            var animatorHumanoid = _target.GetDeckComponent<IDeckAnimationSetTrigger>();
            animatorHumanoid.Trigger(DeckConstantsAnimation.GetUp);

            _target.GetDeckComponent<DeckComponentMovement>().SetEnabled();

            await UniTask.Delay(500, cancellationToken: token);
            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new DeckCommandGetUpSaveData(_target.PrefabId.ID));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<DeckCommandGetUpSaveData>(saveData);
            _target = Deck.GetService<DeckServiceFinder>().GetAgent(loadedData.targetId) as DeckAgentHumanoid;
        }

        private struct DeckCommandGetUpSaveData
        {
            public int targetId;

            public DeckCommandGetUpSaveData(int targetId)
            {
                this.targetId = targetId;
            }
        }
    }
}