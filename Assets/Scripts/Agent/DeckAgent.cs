using System;
using Deck.Data.Agent;
using Deck.Test.Markers;
using UnityEngine;

namespace Deck.Player
{
    public class DeckAgent : MonoBehaviour
    {
        public virtual DeckAgentData GetAgentData()
        {
            throw new NotImplementedException();
        }

        public virtual void Die()
        {
        }
    }
}