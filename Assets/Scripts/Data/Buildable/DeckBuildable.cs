using System;
using System.Linq;
using Deck;
using Deck.Data.Item;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Deck.Data.Buildable
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable", fileName = "Deck Data Buildable")]
    public class DeckBuildable : ScriptableObject
    {
        [FormerlySerializedAs("items"), SerializeField, InfoBox("Duplicate item exists", nameof(CheckUniqueness))] private DeckItemRequirement[] requirements;
        [SerializeField] private DeckAgent agent;
        [SerializeField] private new string name;
        [SerializeField] private SilouetteData[] silouetteData;

        private bool CheckUniqueness()
        {
            if (requirements == null || requirements.Length < 1)
            {
                return true;
            }

            var distinctItemCount = requirements.Select(item => item.Item).Distinct().Count();
            return distinctItemCount != requirements.Length;
        }

        [Button]
        private void CollectSilouetteData(DeckAgent agentToCollect)
        {
            if (agentToCollect.transform.position != Vector3.zero)
            {
                throw new Exception("Agent is not centered");
            }

            var filters = agentToCollect.GetComponentsInChildren<MeshFilter>();
            silouetteData = new SilouetteData[filters.Length];
            for (var i = 0; i < filters.Length; i++)
            {
                var filter = filters[i];
                silouetteData[i] = new SilouetteData(filter.sharedMesh, filter.transform.position, filter.transform.eulerAngles);
            }
        }


        [Serializable]
        public class SilouetteData
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

        public DeckItemRequirement[] GetMaterials() => requirements;
        public DeckAgent GetAgent() => agent;
        public string GetName() => name;
        public SilouetteData[] GetSilouette() => silouetteData;
    }
}