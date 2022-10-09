using UnityEngine;
using Grid = Deck.Map.Grid;

namespace Deck.Services.Implementations.GridService
{
    public class GridService : ServiceBase
    {
        public override void Initialize()
        {
            base.Initialize();
            Debug.LogError("Initializing grid service");
        }

        public Grid GenerateGrid(int width, int height)
        {
            return new Grid(width, height);
        }
    }
}