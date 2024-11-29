using System;
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

        public static DeckRuntimeOrder Create(DeckOrder order, DeckAgent requestingAgent)
        {
            return new DeckRuntimeOrder(order.RequiredAgentTag, order.OrderType, order.Amount, order.Price, requestingAgent);
        }

        private DeckRuntimeOrder(DeckActionTag requiredAgentTag, DeckOrderType orderType, int amount, DeckPrice[] price, DeckAgent requestingAgent)
        {
            this.requiredAgentTag = requiredAgentTag;
            this.orderType = orderType;
            this.amount = amount;
            this.price = price;
            this.requestingAgent = requestingAgent;
        }
    }
}