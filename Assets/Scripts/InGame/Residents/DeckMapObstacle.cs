using Deck.Components;
using Deck.Data.Component;
using UnityEngine;
using Zenject;

namespace Deck.Map.Residents
{
    public class DeckMapObstacle : MonoBehaviour
    {
        [SerializeField] private DeckDataHealth dataHealth;
        public DeckHealthComponent _healthComponent;

        [Inject]
        private void Inject(DeckHealthComponent healthComponent)
        {
            _healthComponent = healthComponent;
        }
    }
}