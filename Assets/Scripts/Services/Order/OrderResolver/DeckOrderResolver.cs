using System;
using Base;
using Commands;
using Commands.Fetch;
using InGame.Agent.Building;
using InGame.Agent.Waiter;
using Services.Finder;
using UI.Notification;
using UnityEngine;
using Utility;
using Utility.Constants;

namespace Services.Order.OrderResolver
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
            var agents = DeckServiceProvider.GetService<DeckServiceFinder>().GetAgentsWithTag(order.RequiredAgentTag);

            DeckAgent closest = null;
            var currentDistance = float.MaxValue;
            var foundAgent = false;
            foreach (var deckAgent in agents)
            {
                var tempDistance = Vector3.Distance(waiter.transform.position, deckAgent.transform.position);
                if (tempDistance < currentDistance)
                {
                    foundAgent = true;
                    closest = deckAgent;
                    currentDistance = tempDistance;
                }
            }

            if (!foundAgent)
            {
                DeckEventNotificationRequested.Create(DeckConstantsNotification.OrderCannotBeFulfilled + " " + order.OrderType + " supporting agent not found").Send();
                return null;
            }

            var building = closest as DeckAgentBuilding;

            var accessPosition = building.GetAccessPosition()[0];


            return new DeckCommandFetchItem(waiter, accessPosition, order.RequestingAgent);
        }
    }
}