using Cysharp.Threading.Tasks;
using Deck.Commands.Sittable;
using Deck.Components;
using Deck.EventManager;
using Deck.InGame.Agent.Customer;
using Deck.Instancing;
using Deck.Services.Tables.Events;
using UnityEngine;
using Zenject;

namespace Deck.Services.Customer
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

        public override void DeInitialize()
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