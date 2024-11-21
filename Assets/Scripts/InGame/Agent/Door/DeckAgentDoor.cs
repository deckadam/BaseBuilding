using Cysharp.Threading.Tasks;
using Deck.Components.Building;
using Deck.Services.Implementations.AreaController.Events;
using Deck.Utility;

namespace Deck.Components.Door
{
    public class DeckAgentDoor : DeckBuilding
    {
        protected override async void AfterInitialize()
        {
            await UniTask.Yield();
            DeckEventOnDoorBuild.Create(transform.position.ToVector2Int()).Send();
        }

        protected override void OnAgentDestroyed()
        {
            var cellPosition = transform.position.ToVector2Int();
            DeckEventOnDoorDestroyed.Create(cellPosition).Send();
        }
    }
}