using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Deck.Components;
using Deck.Save;
using Deck.Services.AgentFinder;
using Deck.Utility;

namespace Deck.Commands
{
    public class DeckCommandDealDamage : DeckCommand
    {
        private bool _canKill;
        private DeckComponentDamageDealer _from;
        private DeckComponentHealth _to;
        private int _damage;
        private float _baseAttackRange;
        private bool _continous;

        public DeckCommandDealDamage()
        {
        }

        public DeckCommandDealDamage(int damage, float baseAttackRange, DeckComponentDamageDealer from, DeckComponentHealth to, bool canKill = true, bool continuous = true)
        {
            _damage = damage;
            _baseAttackRange = baseAttackRange;
            _canKill = canKill;
            _to = to;
            _from = from;
            _continous = continuous;
        }

        public override async UniTask<bool> ProcessCommand(CancellationToken token)
        {
            if (_from == null)
            {
                DeckLogger.Inform("No from agent");
                return false;
            }

            var movementComponent = _from.GetAgent().GetDeckComponent<DeckComponentMovement>();

            if (_to == null)
            {
                DeckLogger.Inform("No to agent");
                return false;
            }

            if (!_continous)
            {
                return await ExecuteDamageDealing(token, movementComponent);
            }

            while (!token.IsCancellationRequested)
            {
                var result = await ExecuteDamageDealing(token, movementComponent);
                if (!result)
                {
                    return false;
                }

                var isCanceled = await UniTask.WaitWhile(() => !_from.CanAttack(), cancellationToken: token).SuppressCancellationThrow();
                if (isCanceled)
                {
                    return false;
                }
            }

            return true;
        }

        private async UniTask<bool> ExecuteDamageDealing(CancellationToken token, DeckComponentMovement movementComponent)
        {
            if (_to.IsDead)
            {
                return false;
            }

            var range = _to.GetAgent().GetItemVisual().GetSize() + _baseAttackRange;

            var isCanceled = await DeckCommandUtility.AwaitTillDestinationIsReached(movementComponent, _to.GetAgent().transform, range, token);
            if (isCanceled)
            {
                return false;
            }

            if (_to.IsDead)
            {
                return false;
            }

            _from.OnAttackStart();
            _from.OnAttack();

            _to.ReduceHealth(_from.GetAgent(), _damage, _canKill);
            return true;
        }

        public override string GetSaveData()
        {
            var saveData = new SaveData(_canKill, _from.GetAgent().UniqueId.ID, _to.GetAgent().UniqueId.ID, _damage, _baseAttackRange, _continous);
            return DeckSaveUtility.GetSerializedData(saveData);
        }

        public override void LoadSaveData(string saveData)
        {
            var loadData = DeckSaveUtility.GetDeserializedData<SaveData>(saveData);
            _canKill = loadData.canKill;
            _from = Deck.GetService<DeckServiceFinder>().GetAgent(loadData.fromAgent).GetDeckComponent<DeckComponentDamageDealer>();
            _to = Deck.GetService<DeckServiceFinder>().GetAgent(loadData.toAgent).GetDeckComponent<DeckComponentHealth>();
            _damage = loadData.damage;
            _baseAttackRange = loadData.baseAttackRange;
            _continous = loadData.continous;
        }

        [Serializable]
        private struct SaveData
        {
            public bool canKill;
            public int fromAgent;
            public int toAgent;
            public int damage;
            public float baseAttackRange;
            public bool continous;

            public SaveData(bool canKill, int fromAgent, int toAgent, int damage, float baseAttackRange, bool continous)
            {
                this.canKill = canKill;
                this.fromAgent = fromAgent;
                this.toAgent = toAgent;
                this.damage = damage;
                this.baseAttackRange = baseAttackRange;
                this.continous = continous;
            }
        }
    }
}