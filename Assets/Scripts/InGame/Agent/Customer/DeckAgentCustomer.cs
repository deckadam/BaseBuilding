using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Commands;
using Deck.Commands.Sittable;
using Deck.Services.Order;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.InGame.Agent.Customer
{
    public class DeckAgentCustomer : DeckAgentHumanoid
    {
        [SerializeField] private DeckOrder order;

        protected override void InternalHumanoidSpawnRequested()
        {
            RequestOrder();
        }

        [Button]
        private void Test()
        {
            Deck.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this, OnOrderCompleted));
        }

        private async void OnOrderCompleted()
        {
            Debug.LogError("Order completed");
            await UniTask.Delay(2000);
            EnqueueCommand(new DeckCommandGetUp(this));

            var leaveCommand = new DeckCommandMove(new Vector3(20, 0, 0), this);
            leaveCommand.OnCompleted += RequestDestroy;
            
            EnqueueCommand(leaveCommand);
        }

        private async void RequestOrder()
        {
            return;
            var destroyToken = gameObject.GetCancellationTokenOnDestroy();
            while (!destroyToken.IsCancellationRequested)
            {
                var result = await UniTask.Delay(10000, cancellationToken: destroyToken).SuppressCancellationThrow();
                if (result)
                {
                    return;
                }

                Deck.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this, OnOrderCompleted));
            }
        }
    }
}