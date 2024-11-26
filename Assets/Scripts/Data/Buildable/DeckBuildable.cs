using System;
using Deck.Base;
using Deck.Data.Currency;
using Deck.Services.Building;
using Deck.Utility;
using Sirenix.OdinInspector;
using UI.Building.BuildMode;
using UnityEngine;

namespace Deck.Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [SerializeField] private DeckId buildingId;
        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private string visibleName;
        [SerializeField] private DeckSilhouetteData[] silhouette;
        [SerializeField] private Vector2Int[] indices;
        [SerializeField] private Vector2Int[] accessIndices;
        [SerializeField] private DeckRotationMode rotationMode;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 extents;
        [SerializeField] private DeckPrice[] prices;
        [SerializeField] private DeckBuildMode buildMode;
        [SerializeField] private DeckBuildable buildableToPlaceOnTop;

        public DeckRotationMode RotationMode => rotationMode;
        public DeckSilhouetteData[] Silhouette => silhouette;
        public int materialCount;
        public Vector2Int[] Indices => indices;
        public Vector2Int[] AccessIndices => accessIndices;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string VisibleName => visibleName;
        public Sprite Icon => icon;
        public Vector3 Extents => extents;
        public DeckPrice[] Prices => prices;
        public DeckBuildMode BuildMode => buildMode;
        public DeckBuildable BuildableToPlaceOnTop => buildableToPlaceOnTop;

        public DeckId BuildingId
        {
            get
            {
                if (buildingId.IsValid)
                {
                    return buildingId;
                }

                throw new Exception("No valid prefab id " + name);
            }
        }

        [Button]
        private void OnValidate()
        {
            CollectSilhouetteData();
            CollectExtentsData();
        }

        public static DeckBuildable Create(string name, Sprite icon, DeckItemVisual representation, DeckAgent agent, DeckBuildMode buildMode)
        {
            var newItem = CreateInstance<DeckBuildable>();
            newItem.name = name;
            newItem.icon = icon;
            newItem.itemVisual = representation;
            newItem.agent = agent;
            newItem.visibleName = name;
            newItem.OnValidate();
            newItem.indices = new[] { Vector2Int.zero };
            newItem.CollectExtentsData();
            newItem.CollectSilhouetteData();
            newItem.buildMode = buildMode;
            newItem.buildingId = DeckId.CreateNew();
            newItem.prices = new[]
            {
                new DeckPrice(0, DeckCurrencyType.Money)
            };
            return newItem;
        }

        private void CollectExtentsData()
        {
            if (itemVisual == null)
            {
                throw new Exception("Item visual not set");
            }

            var bounds = new Bounds();
            foreach (var renderer in itemVisual.GetComponentsInChildren<MeshRenderer>())
            {
                bounds.Encapsulate(renderer.bounds);
            }

            extents = bounds.size;
        }

        private void CollectSilhouetteData()
        {
            if (agent == null)
            {
                throw new Exception("Agent not set");
            }

            if (agent.transform.position != Vector3.zero)
            {
                DeckLogger.Error("Agent is not centered", agent.gameObject);
                throw new Exception("Agent is not centered");
            }


            var filters = itemVisual.GetComponentsInChildren<MeshFilter>();
            silhouette = new DeckSilhouetteData[filters.Length];
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                silhouette[i] = new DeckSilhouetteData(filter.sharedMesh, filter.transform.position, filter.transform.eulerAngles);
            }
        }

        public void SetAgent(DeckAgent agentPrefab)
        {
            agent = agentPrefab;
        }

        public void SetItemVisual(DeckItemVisual itemVisualPrefab)
        {
            itemVisual = itemVisualPrefab;
        }

        public override bool Equals(object other)
        {
            var otherBuildable = other as DeckBuildable;
            return buildingId.Equals(otherBuildable.buildingId);
        }
    }
}