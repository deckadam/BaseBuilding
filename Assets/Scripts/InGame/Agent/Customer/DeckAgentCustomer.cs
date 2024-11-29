using Cysharp.Threading.Tasks;
using Deck.Base;
using Deck.Services.Order;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.InGame.Agent.Customer
{
    public class DeckAgentCustomer : DeckAgentHumanoid
    {
        [SerializeField] private DeckOrder order;

        protected override void AfterInitializationCompleted()
        {
            RequestOrder();
        }

        [Button]
        private void Test()
        {
            Deck.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this));
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

                Deck.GetService<DeckServiceOrder>().RegisterNewOrder(DeckRuntimeOrder.Create(order, this));
            }
        }
    }
}