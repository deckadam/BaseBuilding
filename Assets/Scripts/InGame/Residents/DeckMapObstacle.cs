using Deck.Commands;
using Deck.Data.Component;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Deck.Residents
{
    public class DeckMapObstacle : MonoBehaviour
    {
        [SerializeField] private DeckDataHealth dataHealth;
        [FormerlySerializedAs("_healthComponent")] public DeckComponentHealth componentHealth;

        [Inject]
        private void Inject(DeckComponentHealth componentHealth)
        {
            this.componentHealth = componentHealth;
        }
    }
}