using System;
using System.Collections.Generic;
using System.Linq;
using Base;
using GameManager.Data.GameSetting.Override;
using Services.Building.Buildable.Data;
using Services.Building.Buildable.Data.Parameter;
using Sirenix.OdinInspector;
using UnityEngine;
using Utility;

namespace Services.Building.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable/Deck Data Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [ValueDropdown("@Utility.Resource.DeckResourceLocator.GetAllCategories()")] [SerializeField] private DeckBuildableCategory category;
        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private string visibleName;
        [SerializeField] private DeckBuildableSilhouette silhouetteData;
        [SerializeReference, TypeFilter("@Utility.Resource.DeckResourceLocator.GetAllParameterTypes()")] private List<DeckBuildableParameter> parameters;

        public DeckBuildableCategory Category => category;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string VisibleName => visibleName;
        public DeckBuildableSilhouette SilhouetteData => silhouetteData;
        private Dictionary<Type, DeckBuildableParameter> _parameters;

#if UNITY_EDITOR
        [Button]
        private void OnValidate()
        {
            CollectSilhouetteData();
            CollectExtentsData();
        }

        public static DeckBuildable Create(string name, DeckItemVisual representation, DeckAgent agent)
        {
            var newItem = CreateInstance<DeckBuildable>();
            newItem.name = name;
            newItem.itemVisual = representation;
            newItem.agent = agent;
            newItem.visibleName = name;
            newItem.OnValidate();
            newItem.CollectSilhouetteData();
            return newItem;
        }

        private Vector3 CollectExtentsData()
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

            return bounds.size;
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
            var tempSilhouette = new DeckSilhouetteData[filters.Length];
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                tempSilhouette[i] = new DeckSilhouetteData(filter.sharedMesh, filter.transform.position, filter.transform.eulerAngles);
            }
            silhouetteData = new DeckBuildableSilhouette(tempSilhouette,CollectExtentsData(),1);
        }
#endif

        public void Initialize()
        {
            _parameters = new Dictionary<Type, DeckBuildableParameter>();
            foreach (var deckBuildableParameter in parameters)
            {
                deckBuildableParameter.ResetOverride();
                _parameters.Add(deckBuildableParameter.GetType(), deckBuildableParameter);
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
            var obj = other as DeckBuildable;
            return obj.agent.PrefabId.Equals(agent.PrefabId);
        }

        protected bool Equals(DeckBuildable other)
        {
            return base.Equals(other) && visibleName == other.visibleName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), visibleName);
        }

        public void ApplyOverride(DeckGameSettingBuildableOverride buildableOverride)
        {
            foreach (var buildableOverrideParameter in buildableOverride.Parameters)
            {
                var match = parameters.First(item => item.ParameterType == buildableOverrideParameter.ParameterType);
                match.ApplyOverride(buildableOverrideParameter);
            }
        }

        public void ResetOverrides()
        {
            foreach (var parameter in parameters)
            {
                parameter.ResetOverride();
            }
        }

        public bool TryGetParameter<T>(out T parameter) where T : DeckBuildableParameter
        {
            if (!_parameters.TryGetValue(typeof(T), out var val))
            {
                DeckLogger.Inform($"Tried to fetch non existent parameter type {typeof(T).Name}  {visibleName}");
                parameter = null;
                return false;
            }

            parameter = val as T;
            return parameter != null;
        }

        public T GetParameter<T>() where T : DeckBuildableParameter
        {
            return _parameters[typeof(T)] as T;
        }
    }

    [Serializable]
    public struct DeckBuildableSilhouette
    {
        [SerializeField] private DeckSilhouetteData[] silhouette;
        [SerializeField] private Vector3 extents;
        [SerializeField] private int materialCount;

        public DeckSilhouetteData[] Silhouette => silhouette;
        public Vector3 Extents => extents;
        public int MaterialCount => materialCount;

        public DeckBuildableSilhouette(DeckSilhouetteData[] silhouette, Vector3 extents, int materialCount)
        {
            this.silhouette = silhouette;
            this.extents = extents;
            this.materialCount = materialCount;
        }
    }
}