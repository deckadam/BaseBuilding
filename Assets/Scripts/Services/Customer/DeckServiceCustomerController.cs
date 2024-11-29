using Deck.Commands.Sit;
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

        private void OnTableAvailable(DeckEventOnTableAvailable obj)
        {
            foreach (var chair in obj.Table.Chairs)
            {
                if (!chair.IsAvailable)
                {
                    continue;
                }

                var newCustomer = CreateCustomer();
                newCustomer.EnqueueCommand(new DeckCommandSit(newCustomer, chair));

                chair.SetOccupied();
            }
        }

        private DeckAgentCustomer CreateCustomer()
        {
            var newCustomer = (DeckAgentCustomer)_instanceProvider.RentAgent(_customerAgentPrefab.PrefabId);
            newCustomer.transform.position = new Vector3(20, 0, 20);
            newCustomer.Initialize();

            return newCustomer;
        }
    }
}