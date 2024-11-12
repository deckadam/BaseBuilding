using System.Collections.Generic;
using System.Linq;
using Deck.Base.Id;
using Deck.Components;
using Deck.Utility;
using Deck.Utility.Iterators;
using UnityEngine;

namespace Deck.ItemVisualProviders
{
    [CreateAssetMenu(fileName = "DeckItemVisualProviderBarTable", menuName = "Service/ItemVisualManager/DeckItemVisualProviderBarTable")]
    public class DeckItemVisualProviderBarTable : DeckItemVisualProviderBasic
    {
        [SerializeField] private DeckItemVisual horizontalPrefab;
        [SerializeField] private DeckItemVisual verticalPrefab;
        [SerializeField] private DeckItemVisual twoCornerUpperLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerUpperRightPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerLeftPrefab;
        [SerializeField] private DeckItemVisual twoCornerLowerRightPrefab;
        [SerializeField] private DeckItemVisual threeCornerUpPrefab;
        [SerializeField] private DeckItemVisual threeCornerDownPrefab;
        [SerializeField] private DeckItemVisual threeCornerLeftPrefab;
        [SerializeField] private DeckItemVisual threeCornerRightPrefab;
        [SerializeField] private DeckItemVisual fourCornerPrefab;
        [SerializeField] private DeckItemVisual emptyPrefab;

        private Dictionary<Vector2Int, DeckItemVisual> _activeBarTables;
        private Dictionary<Vector2Int, DeckAgent> _activeBarTableAgents;
        private HashSet<Vector2Int> _barTableCheckSet;

        protected override void OnInitialize()
        {
            _activeBarTables = new Dictionary<Vector2Int, DeckItemVisual>();
            _activeBarTableAgents = new Dictionary<Vector2Int, DeckAgent>();
            _barTableCheckSet = new HashSet<Vector2Int>();
        }

        public override bool ReturnItemVisual(DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(itemVisual)) return false;

            ReturnIfHasItemVisual(itemVisual);
            var itemPos = itemVisual.transform.position.ToVector2Int();

            _activeBarTables.Remove(itemPos);
            _activeBarTableAgents.Remove(itemPos);
            _barTableCheckSet.Remove(itemPos);

            return true;
        }

        public override bool RequestItemVisual(DeckAgent agent, DeckId prefabId, Vector2Int cellIndex, out DeckItemVisual itemVisual)
        {
            if (!IsSupportedItemVisual(prefabId))
            {
                itemVisual = null;
                return false;
            }

            _activeBarTableAgents[cellIndex] = agent;
            _barTableCheckSet.Add(cellIndex);
            itemVisual = PlaceBarTableWithNeighbours(cellIndex);
            return true;
        }

        private DeckItemVisual PlaceBarTableWithNeighbours(Vector2Int position)
        {
            var neighbours = position.GetNeighbours();

            foreach (var neighbour in neighbours)
            {
                if (!_barTableCheckSet.Contains(neighbour)) continue;
                ReturnIfHasItemVisual(_activeBarTables[neighbour]);
                PlaceItemVisual(neighbour);
            }

            return PlaceItemVisual(position);
        }

        private DeckItemVisual PlaceItemVisual(Vector2Int position)
        {
            var checkList = new bool[4];

            var neighbours = position.GetNeighbours();
            for (var index = 0; index < neighbours.Length; index++)
            {
                var neighbour = neighbours[index];
                var isBarTable = _barTableCheckSet.Contains(neighbour);
                checkList[index] = isBarTable;
            }

            var agent = _activeBarTableAgents[position];
            var itemVisualPrefab = GetVisualToPlace(checkList);
            RentIfHasItemVisual(itemVisualPrefab.PrefabId, out var itemVisualInstance);
            agent.SetItemVisual(itemVisualInstance);
            itemVisualInstance.SetAgent(agent);

            _activeBarTables[position] = itemVisualInstance;

            return itemVisualInstance;
        }

        private DeckItemVisual GetVisualToPlace(IReadOnlyList<bool> checkList)
        {
            var trueCount = checkList.Count(b => b);

            if (trueCount == 4)
            {
                return fourCornerPrefab;
            }

            if (trueCount == 3)
            {
                if (!checkList[0])
                {
                    Debug.LogError("1");
                    return threeCornerLeftPrefab;
                }

                if (!checkList[1])
                {
                    Debug.LogError("2");
                    return threeCornerRightPrefab;
                }

                if (!checkList[2])
                {
                    Debug.LogError("3");

                    return threeCornerDownPrefab;
                }

                if (!checkList[3])
                {
                    Debug.LogError("4");

                    return threeCornerUpPrefab;
                }

                DeckLogger.Error("Huh!!!!");
            }

            if (trueCount == 2)
            {
                if (checkList[0] && checkList[2])
                {
                    Debug.LogError("5");

                    return twoCornerUpperRightPrefab;
                }

                if (checkList[0] && checkList[3])
                {
                    Debug.LogError("6");

                    return twoCornerLowerRightPrefab;
                }

                if (checkList[1] && checkList[2])
                {
                    Debug.LogError("7");

                    return twoCornerUpperLeftPrefab;
                }

                if (checkList[1] && checkList[3])
                {
                    Debug.LogError("8");

                    return twoCornerLowerLeftPrefab;
                }

                if (checkList[0] && checkList[1])
                {
                    Debug.LogError("horizontal");
                    return horizontalPrefab;
                }

                if (checkList[2] && checkList[3])
                {
                    Debug.LogError("vertical");
                    return verticalPrefab;
                }

                DeckLogger.Error("Huh!!!!");
            }

            if (trueCount == 1)
            {
                if (checkList[0] || checkList[1])
                {
                    return horizontalPrefab;
                }

                if (checkList[2] || checkList[3])
                {
                    return verticalPrefab;
                }
            }

            return emptyPrefab;
        }
    }
}