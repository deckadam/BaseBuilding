using Base;
using UnityEngine;

namespace General
{
    public interface IDeckSittable
    {
        void OnSit(DeckAgentHumanoid humanoid);
        void OnGetUp(DeckAgentHumanoid humanoid);
        Vector3 GetSitPosition();
    }
}