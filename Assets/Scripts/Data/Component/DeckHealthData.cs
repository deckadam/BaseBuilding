using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Data.Component
{
    [CreateAssetMenu(menuName = "Deck/Component/HealthComponent", fileName = "Deck Health Component Data")]
    public class DeckHealthData : ScriptableObject
    {
        public int Health
        {
            get => health;
        }

        [SerializeField] private int health;
    }
}