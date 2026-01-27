using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using Data.Buildable.Data;
using Data.Buildable.Data.Parameter;
using Data.Currency;
using GameManager.Data.GameSetting.Override;
using InGame.Agent.Building;
using Services.Building;
using Sirenix.OdinInspector;
using UI.Building.BuildMode;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable/Deck Data Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [SerializeReference, TypeFilter(nameof(GetAllParameterTypes))] private List<DeckBuildableParameter> parameters;
        [ValueDropdown(nameof(GetAllCategories))] [SerializeField] private DeckBuildableCategory category;

        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private string visibleName;
        [SerializeField] private DeckSilhouetteData[] silhouette;
        [SerializeField] private DeckRotationMode rotationMode;
        [SerializeField] private Vector3 extents;
        [SerializeField] private DeckPrice[] prices;
        [SerializeField] private DeckBuildMode buildMode;
        [SerializeField] private DeckBuildable buildableToPlaceOnTop;
        [SerializeField] private int materialCount;
        [SerializeField] private bool isGridBased;

        [SerializeField, ShowIf(nameof(isGridBased))] private Vector2Int[] indices;

        [SerializeField, ShowIf(nameof(isGridBased))] private Vector2Int[] accessIndices;

        [SerializeField] private bool isLimited;

        [SerializeField, ShowIf(nameof(isLimited))] private int limit;

        public DeckBuildableCategory Category => category;
        public DeckRotationMode RotationMode => rotationMode;
        public DeckSilhouetteData[] Silhouette => silhouette;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string VisibleName => visibleName;
        public Vector3 Extents => extents;
        public int MaterialCount => materialCount;
        public DeckBuildMode BuildMode => buildMode;
        public DeckBuildable BuildableToPlaceOnTop => buildableToPlaceOnTop;
        public bool IsGridBased => isGridBased;
        public Vector2Int[] Indices => indices;
        public Vector2Int[] AccessIndices => accessIndices;
        public bool IsLimited => isLimited;

        public DeckPrice[] Prices
        {
            get
            {
                if (_isPriceOverriden)
                {
                    return _overridenPrice;
                }

                return prices;
            }
        }

        public int Limit
        {
            get
            {
                if (_isLimitOverriden)
                {
                    return _overridenLimit;
                }

                return limit;
            }
        }

        private int _currentLimit;

        private bool _isLimitOverriden;
        private int _overridenLimit;

        private bool _isPriceOverriden;
        private DeckPrice[] _overridenPrice;


        private Dictionary<Type, DeckBuildableParameter> _parameters;

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


        public void Initialize()
        {
            _parameters = new Dictionary<Type, DeckBuildableParameter>();
            foreach (var deckBuildableParameter in parameters)
            {
                _parameters.Add(deckBuildableParameter.GetType(), deckBuildableParameter);
            }
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

        public void ApplyOverride(DeckGameSettingBuildableOverride buildableOverride)
        {
            if (buildableOverride.OverrideLimit)
            {
                _isLimitOverriden = true;
                _overridenLimit = buildableOverride.OverridenLimit;
            }
            else
            {
                _isLimitOverriden = false;
            }

            if (buildableOverride.OverridePrice)
            {
                _isPriceOverriden = true;
                _overridenPrice = buildableOverride.OverridenPrice;
            }
            else
            {
                _isPriceOverriden = false;
            }
        }

        public void ResetOverrides()
        {
            _isLimitOverriden = false;
            _isPriceOverriden = false;
        }

        public bool TryGetParameter<T>(out T parameter) where T : DeckBuildableParameter
        {
            if (!_parameters.TryGetValue(typeof(T), out var val))
            {
                DeckLogger.Warning($"Tried to fetch non existent parameter type {typeof(T).Name}");
                parameter = null;
                return false;
            }

            parameter = val as T;
            return parameter != null;
        }

        #region Editor Stuff

        private IEnumerable<DeckBuildableCategory> GetAllCategories()
        {
            return AssetDatabase.FindAssets($"t:{nameof(DeckBuildableCategory)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<DeckBuildableCategory>)
                .OrderBy(x => x.name);
        }

        private IEnumerable<Type> GetAllParameterTypes()
        {
            return typeof(DeckBuildableParameter).Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(DeckBuildableParameter)) && !t.IsAbstract);
        }

        #endregion
    }
}