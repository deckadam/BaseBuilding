using System;
using System.Linq;
using Deck.Agent;
using Deck.Data.Item;
using Deck.Item;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [SerializeField, InfoBox("Duplicate item exists", nameof(CheckUniqueness))]
        private DeckItemRequirement[] requirements;

        [SerializeField] private DeckAgent agent;
        [SerializeField] private DeckItemVisual itemVisual;
        [SerializeField] private new string name;
        [SerializeField] private SilouetteData[] silouette;
        [SerializeField] private Vector2Int[] indices;
        [SerializeField] private bool rotatable;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 extents;

        public DeckItemRequirement[] Requeriements => requirements;
        public SilouetteData[] Silouette => silouette;
        public Vector2Int[] Indices => indices;
        public bool Rotatable => rotatable;
        public DeckAgent Agent => agent;
        public DeckItemVisual ItemVisual => itemVisual;
        public string Name => name;
        public Sprite Icon => icon;
        public Vector3 Extents => extents;

        [Button]
        private void OnValidate()
        {
            CollectSilouetteData();
            CollectExtentsData();
            CheckUniqueness();
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

        private bool CheckUniqueness()
        {
            if (requirements == null || requirements.Length < 1)
            {
                return true;
            }

            var distinctItemCount = requirements.Select(item => item.Item).Distinct().Count();
            return distinctItemCount != requirements.Length;
        }

        private void CollectSilouetteData()
        {
            if (agent == null)
            {
                throw new Exception("Agent not set");
            }

            if (agent.transform.position != Vector3.zero)
            {
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
    }
}