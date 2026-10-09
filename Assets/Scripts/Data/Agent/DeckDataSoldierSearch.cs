using UnityEngine;

namespace Data.Agent.Search
{
    [CreateAssetMenu(fileName = "Deck Data Soldier Search", menuName = "Deck/Data/Component/Search", order = 0)]
    public class DeckDataSoldierSearch : DeckDataAgent
    {
        [SerializeField] private float searchRadius;

        public float SearchRadius => searchRadius;
    }
}