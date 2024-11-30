using Deck.Base;
using UnityEngine;

namespace Deck.General
{
    public interface IDeckSittable
    {
        void OnSit(DeckAgentHumanoid humanoid);
        void OnGetUp(DeckAgentHumanoid humanoid);
        Vector3 GetSitPosition();
    }
}