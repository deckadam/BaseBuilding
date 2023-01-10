using Sirenix.OdinInspector;
using UnityEngine;

namespace Deck.Test.Data.Component
{
    [CreateAssetMenu(menuName = "Deck/Data/HealthComponent", fileName = "Deck Health Component Data")]
    public class DeckHealthComponentData : ScriptableObject
    {
        [ShowInInspector] public int health { get; private set; }
    }
}