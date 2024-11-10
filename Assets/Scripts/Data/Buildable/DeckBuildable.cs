using System;
using Deck.Components;
using Deck.Data.Currency;
using Deck.Utility.Logger;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private new string name;
        [SerializeField] private string visibleName;
        [SerializeField] private SilouetteData[] silouette;
        [SerializeField] private Vector2Int[] indices;
        [SerializeField] private bool rotatable;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 extents;
        [SerializeField] private DeckPrice[] prices;
        [SerializeField] private bool canBeHangedToWall;
        [SerializeField] private bool canBePlacedOnTopOfAnotherObject;

        public SilouetteData[] Silouette => silouette;
        public int materialCount;
        public Vector2Int[] Indices => indices;
        public bool Rotatable => rotatable;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string Name => name;
        public string VisibleName => visibleName;
        public Sprite Icon => icon;
        public Vector3 Extents => extents;
        public DeckPrice[] Prices => prices;
        public bool CanBeHangedToWall => canBeHangedToWall;
        public bool CanBePlacedOnTopOfAnotherObject => canBePlacedOnTopOfAnotherObject;

        [Button]
        private void OnValidate()
        {
            CollectSilhouetteData();
            CollectExtentsData();
        }

        public static DeckBuildable Create(string name, Sprite icon, DeckItemVisual representation, DeckAgent agent, bool rotatable, bool canBeHangedToWall)
        {
            var newItem = CreateInstance<DeckBuildable>();
            newItem.name = name;
            newItem.icon = icon;
            newItem.itemVisual = representation;
            newItem.agent = agent;
            newItem.visibleName = name;
            newItem.rotatable = rotatable;
            newItem.canBeHangedToWall = canBeHangedToWall;
            newItem.OnValidate();
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
            silouette = new SilouetteData[filters.Length];
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                silouette[i] = new SilouetteData(filter.sharedMesh, filter.transform.position, filter.transform.eulerAngles);
            }
        }

        [Serializable]
        public struct SilouetteData
        {
            [SerializeField] private Mesh mesh;
            [SerializeField] private Vector3 position;
            [SerializeField] private Vector3 rotation;

            public SilouetteData(Mesh mesh, Vector3 position, Vector3 rotation)
            {
                this.mesh = mesh;
                this.position = position;
                this.rotation = rotation;
            }

            public Mesh GetMesh() => mesh;
            public Vector3 GetPosition() => position;
            public Vector3 GetRotation() => rotation;
        }

        public void SetAgent(DeckAgent agentPrefab)
        {
            agent = agentPrefab;
        }

        public void SetItemVisual(DeckItemVisual itemVisualPrefab)
        {
            itemVisual = itemVisualPrefab;
        }
    }
}