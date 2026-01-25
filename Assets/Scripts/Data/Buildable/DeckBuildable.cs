using System;
using Base;
using Data.Currency;
using InGame.Agent.Building;
using Services.Building;
using Sirenix.OdinInspector;
using UI.Building.BuildMode;
using UnityEngine;
using Utility;

namespace Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private string visibleName;
        [SerializeField] private DeckSilhouetteData[] silhouette;
        [SerializeField] private DeckRotationMode rotationMode;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 extents;
        [SerializeField] private DeckPrice[] prices;
        [SerializeField] private DeckBuildMode buildMode;
        [SerializeField] private DeckBuildable buildableToPlaceOnTop;
        [SerializeField] private int materialCount;
        [SerializeField] private bool isGridBased;

        [SerializeField, ShowIf(nameof(isGridBased))]
        private Vector2Int[] indices;

        [SerializeField, ShowIf(nameof(isGridBased))]
        private Vector2Int[] accessIndices;

        [SerializeField] private bool isLimited;

        [SerializeField, ShowIf(nameof(isLimited))]
        private int limit;

        public DeckRotationMode RotationMode => rotationMode;
        public DeckSilhouetteData[] Silhouette => silhouette;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string VisibleName => visibleName;
        public Sprite Icon => icon;
        public Vector3 Extents => extents;
        public int MaterialCount => materialCount;
        public DeckPrice[] Prices => prices;
        public DeckBuildMode BuildMode => buildMode;
        public DeckBuildable BuildableToPlaceOnTop => buildableToPlaceOnTop;
        public bool IsGridBased => isGridBased;
        public Vector2Int[] Indices => indices;
        public Vector2Int[] AccessIndices => accessIndices;
        public bool IsLimited => isLimited;
        public int Limit => limit;

        private int _currentLimit;

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
            newItem.CollectExtentsData();
            newItem.CollectSilhouetteData();
            newItem.buildMode = buildMode;
            newItem.buildableToPlaceOnTop = null;

            newItem.prices = new[]
            {
                new DeckPrice(0, DeckCurrencyType.Money)
            };
            newItem.indices = new[] { Vector2Int.zero };
            newItem.accessIndices = new[] { Vector2Int.zero };

            newItem.isLimited = false;
            newItem.limit = int.MaxValue;
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
            var otherBuildable = other as DeckAgentBuilding;
            return agent.PrefabId.Equals(otherBuildable.PrefabId);
        }

        protected bool Equals(DeckBuildable other)
        {
            return base.Equals(other) && visibleName == other.visibleName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), visibleName);
        }

        public void SetCurrentLimit(int maxLimit)
        {
            _currentLimit = maxLimit;
        }

        public int GetCurrentLimit()
        {
            return _currentLimit;
        }
    }
}