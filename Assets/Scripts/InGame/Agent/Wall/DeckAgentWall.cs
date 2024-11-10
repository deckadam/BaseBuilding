using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.Services.Implementations.AreaController.Events;
using Deck.Utility;
using Deck.Utility.Logger;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Components.Wall
{
    public class DeckAgentWall : DeckBuilding
    {
        protected override void AfterInitialize()
        {
            DeckEventOnWallBuild.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void OnAgentDestroyed()
        {
            DeckEventOnWallDestroyed.Create(transform.position.ToVector2Int()).Send();
        }
    }
}