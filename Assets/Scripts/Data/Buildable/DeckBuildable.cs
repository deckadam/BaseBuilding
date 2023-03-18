using System.Linq;
using Deck.Agent;
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

        private bool CheckUniqueness()
        {
            if (requirements == null || requirements.Length < 1)
            {
                return true;
            }

            var distinctItemCount = requirements.Select(item => item.item).Distinct().Count();
            return distinctItemCount != requirements.Length;
        }

        public DeckItemRequirement[] GetMaterials() => requirements;
        public DeckAgent GetAgent() => agent;
        public string GetName() => name;
    }
}