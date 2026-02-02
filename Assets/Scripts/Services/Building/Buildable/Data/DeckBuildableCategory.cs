using UnityEngine;

namespace Services.Building.Buildable.Data
{
    [CreateAssetMenu(menuName = "Deck/Data/Buildable/Buildable Category", fileName = "Deck Data Buildable Category")]
    public class DeckBuildableCategory : ScriptableObject
    {
        [SerializeField] private string categoryName;

        public string CategoryName => categoryName;
    }
}