using Commands.Sittable;
using Components.Movement;
using Cysharp.Threading.Tasks;
using EventManager;
using InGame.Agent.Customer;
using Instancing;
using Services.Tables.Events;
using UnityEngine;
using Zenject;

namespace Services.Customer
{
    public class DeckServiceCustomerController : DeckServiceBase
    {
        [SerializeField] private DeckAgentCustomer _customerAgentPrefab;
        private DeckInstanceProvider _instanceProvider;

        [Inject]
        private void Inject(DeckInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;
        }

        public override void Initialize()
        {
            DeckEventManager.Register<DeckEventOnTableAvailable>(OnTableAvailable);
        }

        protected override void DeInitialize()
        {
            DeckEventManager.Unregister<DeckEventOnTableAvailable>(OnTableAvailable);
        }

        private async void OnTableAvailable(DeckEventOnTableAvailable obj)
        {
            foreach (var chair in obj.Table.Chairs)
            {
                if (!chair.IsAvailable)
                {
                    continue;
                }

                var newCustomer = await CreateCustomer();
                newCustomer.EnqueueCommand(new DeckCommandSit(newCustomer, chair));

                chair.SetOccupied();
            }
        }

        private async UniTask<DeckAgentCustomer> CreateCustomer()
        {
            var newCustomer = (DeckAgentCustomer)_instanceProvider.RentAgent(_customerAgentPrefab.PrefabId);
            newCustomer.Initialize();
            await UniTask.NextFrame();
            newCustomer.GetDeckComponent<DeckComponentMovement>().SetPosition(new Vector3(10, 0, 10));
            await UniTask.NextFrame();
            newCustomer.GetDeckComponent<DeckComponentMovement>().SetPosition(new Vector3(10, 0, 10));

            return newCustomer;
        }
    }
}