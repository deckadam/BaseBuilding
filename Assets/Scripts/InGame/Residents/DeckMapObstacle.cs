using Deck.Components;
using Deck.Data.Component;
using UnityEngine;
using Zenject;

namespace Deck.Residents
{
    public class DeckMapObstacle : MonoBehaviour
    {
        [SerializeField] private DeckComponentHealth componentHealth;
        [SerializeField] private DeckDataHealth dataHealth;

        [Inject]
        private void Inject(DeckComponentHealth componentHealth)
        {
            this.componentHealth = componentHealth;
        }
    }
}