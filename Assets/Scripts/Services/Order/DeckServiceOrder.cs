using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using InGame.Agent.Waiter;
using Services.Order.OrderResolver;
using Services.Waiter;
using UnityEngine;

namespace Services.Order
{
    public class DeckServiceOrder : DeckServiceBase
    {
        private Queue<DeckRuntimeOrder> _nonProcessedOrders = new();
        private CancellationTokenSource _tokenSource;
        private DeckServiceWaiter _waiterService;

        public override void Initialize()
        {
            _waiterService = DeckServiceProvider.GetService<DeckServiceWaiter>();
        }

        public override void BeforeGameSessionInitialized()
        {
            _nonProcessedOrders.Clear();
            _tokenSource = new CancellationTokenSource();

            ProcessOrders();
        }

        public override void BeforeGameSessionDeinitialized()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = null;
        }

        public void RegisterNewOrder(DeckRuntimeOrder newOrder)
        {
            Debug.LogError("Register");
            _nonProcessedOrders.Enqueue(newOrder);
        }

        private bool TryGetNextOrder(out DeckRuntimeOrder order)
        {
            return _nonProcessedOrders.TryDequeue(out order);
        }

        private async void ProcessOrders()
        {
            while (!_tokenSource.IsCancellationRequested)
            {
                var result = await UniTask.NextFrame(cancellationToken: _tokenSource.Token).SuppressCancellationThrow();

                if (result)
                {
                    return;
                }

                if (!TryGetNextOrder(out var orderToProcess))
                {
                    continue;
                }

                if (!_waiterService.TryGetAvailableWaiter(out DeckAgentWaiter availableWaiter))
                {
                    continue;
                }

                var order = DeckOrderResolver.GetCommandFromOrder(orderToProcess, availableWaiter);
                if (order == null)
                {
                    _waiterService.RegisterAvailableWaiter(availableWaiter);
                    continue;
                }

                order.OnCompleted = orderToProcess.OnOrderCompleted;

                availableWaiter.ProcessOrder(order);
            }
        }
    }
}