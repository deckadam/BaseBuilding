using Base;
using Commands;
using Commands.Sittable;
using Cysharp.Threading.Tasks;
using Services;
using Services.Order;
using UnityEngine;

namespace InGame.Agent.Customer
{
    public class DeckAgentCustomer : DeckAgentHumanoid
    {
        [SerializeField] private DeckOrder order;

        protected override void InternalHumanoidSpawnRequested()
        {
            RequestOrder();
        }

        private void Test()
        {
            DeckServiceProvider.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this, OnOrderCompleted));
        }

        private async void OnOrderCompleted()
        {
            await UniTask.Delay(2000);
            EnqueueCommand(new DeckCommandGetUp(this));

            var leaveCommand = new DeckCommandMove(new Vector3(20, 0, 0), this).RegisterToOnCompleted(RequestDestroy);

            EnqueueCommand(leaveCommand);
        }

        private async void RequestOrder()
        {
            // var destroyToken = gameObject.GetCancellationTokenOnDestroy();
            // while (!destroyToken.IsCancellationRequested)
            // {
                // var result = await UniTask.Delay(10000, cancellationToken: destroyToken).SuppressCancellationThrow();
                // if (result)
                // {
                    // return;
                // }

                // DeckServiceProvider.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this, OnOrderCompleted));
            // }
            await UniTask.CompletedTask;
        }
    }
}