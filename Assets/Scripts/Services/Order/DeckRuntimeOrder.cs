using System;
using Base;
using Deck.Base;
using Deck.Data.Currency;
using UnityEngine;

namespace Deck.Services.Order
{
    [Serializable]
    public struct DeckRuntimeOrder
    {
        [SerializeField] private DeckActionTag requiredAgentTag;
        [SerializeField] private DeckOrderType orderType;
        [SerializeField] private int amount;
        [SerializeField] private DeckPrice[] price;
        [SerializeField] private DeckAgent requestingAgent;

        public DeckActionTag RequiredAgentTag => requiredAgentTag;
        public DeckOrderType OrderType => orderType;
        public int Amount => amount;
        public DeckPrice[] Price => price;
        public DeckAgent RequestingAgent => requestingAgent;
        public Action OnOrderCompleted { get; }

        public static DeckRuntimeOrder Create(DeckOrder order, DeckAgent requestingAgent,Action onOrderCompleted)
        {
            return new DeckRuntimeOrder(order.RequiredAgentTag, order.OrderType, order.Amount, order.Price, requestingAgent,onOrderCompleted);
        }

        private DeckRuntimeOrder(DeckActionTag requiredAgentTag, DeckOrderType orderType, int amount, DeckPrice[] price, DeckAgent requestingAgent,Action onOrderCompleted)
        {
            this.requiredAgentTag = requiredAgentTag;
            this.orderType = orderType;
            this.amount = amount;
            this.price = price;
            this.requestingAgent = requestingAgent;
            OnOrderCompleted = onOrderCompleted;
        }
    }
}