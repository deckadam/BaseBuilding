using System;
using System.Threading;
using Base;
using Components.AnimatingComponents.Interfaces;
using Components.Movement;
using Cysharp.Threading.Tasks;
using General;
using Services.Finder;
using Systems.SystemSave;
using Utility.Constants;

namespace Commands.Sittable
{
    public class DeckCommandSit : DeckCommand
    {
        private DeckAgent _target;
        private DeckAgent _sittable;

        public DeckCommandSit(DeckAgent target, DeckAgent sittable)
        {
            _target = target;
            _sittable = sittable;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            await new DeckCommandMove(_sittable.transform.position, _target).ProcessCommand(token);

            var sittable = _sittable.GetComponent<IDeckSittable>();

            _target.GetDeckComponent<DeckComponentMovement>().SetDisabled();
            _target.transform.position = _sittable.transform.TransformPoint(sittable.GetSitPosition());
            _target.transform.rotation = _sittable.transform.rotation;

            var animatorHumanoid = _target.GetDeckComponent<IDeckAnimationSetTrigger>();
            animatorHumanoid.Trigger(DeckConstantsAnimation.Sit);

            sittable.OnSit((DeckAgentHumanoid)_target);

            await UniTask.Delay(1000, cancellationToken: token);
            return true;
        }

        public override string GetSaveData()
        {
            return DeckSaveUtility.GetSerializedData(new DeckCommandSitSaveData(_target.UniqueId.ID, _sittable.UniqueId.ID));
        }

        public override void LoadSaveData(string data)
        {
            var deserializedData = DeckSaveUtility.GetDeserializedData<DeckCommandSitSaveData>(data);
            _target = Services.DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(deserializedData.targetId);
            _sittable = Services.DeckServiceProvider.GetService<DeckServiceFinder>().GetAgent(deserializedData.sittableId);
        }

        [Serializable]
        private struct DeckCommandSitSaveData
        {
            public int targetId;
            public int sittableId;

            public DeckCommandSitSaveData(int targetId, int sittableId)
            {
                this.targetId = targetId;
                this.sittableId = sittableId;
            }
        }
    }
}