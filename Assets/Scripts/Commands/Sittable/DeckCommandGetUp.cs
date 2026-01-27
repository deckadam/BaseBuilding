using System.Threading;
using Base;
using Components.AnimatingComponents.Interfaces;
using Components.Movement;
using Cysharp.Threading.Tasks;
using Services.Finder;
using Systems.SystemSave;
using Utility.Constants;

namespace Commands.Sittable
{
    public class DeckCommandGetUp : DeckCommand
    {
        private DeckAgentHumanoid _target;

        public DeckCommandGetUp(DeckAgentHumanoid target)
        {
            _target = target;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            var animatorHumanoid = _target.GetDeckComponent<IDeckAnimationSetTrigger>();
            animatorHumanoid.Trigger(DeckConstantsAnimation.GetUp);

            _target.GetDeckComponent<DeckComponentMovement>().SetEnabled();

            await UniTask.Delay(500, cancellationToken: token);
            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new DeckCommandGetUpSaveData(_target.PrefabId.Id));
        }

        public override void LoadSaveData(string saveData)
        {
            var loadedData = DeckSaveUtility.GetDeserializedData<DeckCommandGetUpSaveData>(saveData);
            _target = Services.DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(loadedData.targetId) as DeckAgentHumanoid;
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