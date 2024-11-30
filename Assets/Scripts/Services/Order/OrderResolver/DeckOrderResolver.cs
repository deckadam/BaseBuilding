using System;
using Deck.Base;
using Deck.Commands;
using Deck.Commands.Fetch;
using Deck.InGame.Agent.Building;
using Deck.InGame.Agent.Waiter;
using Deck.Services.Finder;
using Deck.UI.Notification;
using Deck.Utility;
using Deck.Utility.Constants;
using UnityEngine;

namespace Deck.Services.Order.OrderResolver
{
    public static class DeckOrderResolver
    {
        public static DeckCommand GetCommandFromOrder(DeckRuntimeOrder order, DeckAgentWaiter waiter)
        {
            switch (order.OrderType)
            {
                case DeckOrderType.FilterCoffee:
                    return GetCoffeeFetchCommand(order, waiter);

                default:
                    throw new Exception("Non implemented deck order type " + order.OrderType);
            }
        }

        private static DeckCommandFetchItem GetCoffeeFetchCommand(DeckRuntimeOrder order, DeckAgentWaiter waiter)
        {
            var agents = Deck.GetService<DeckServiceFinder>().GetAgentsWithTag(order.RequiredAgentTag);

            DeckAgent closest = null;
            var currentDistance = float.MaxValue;
            var foundAgent = false;
            foreach (var deckAgent in agents)
            {
                if (Vector3.Distance(waiter.transform.position, deckAgent.transform.position) < currentDistance)
                {
                    foundAgent = true;
                    closest = deckAgent;
                }
            }

            if (!foundAgent)
            {
                DeckEventNotificationRequested.Create(DeckConstantsNotification.OrderCannotBeFulfilled+" " + order.OrderType +" supporting agent not found").Send();
                return null;
            }

            var building = closest as DeckBuilding;

            var accessPosition = building.GetAccessPosition()[0];


            return new DeckCommandFetchItem(waiter, accessPosition, order.RequestingAgent);
        }
    }
}