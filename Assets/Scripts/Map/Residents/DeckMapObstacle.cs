using Deck.Components;
using Deck.Data.Component;
using Deck.Test.Markers;
using UnityEngine;
using Zenject;

namespace Deck.Map.Residents
{
    public class DeckMapObstacle : MonoBehaviour, IDeckDamagable
    {
        [SerializeField] private DeckHealthData _healthData;
        public DeckHealthComponent _healthComponent;

        [Inject]
        private void Inject(DeckHealthComponent healthComponent)
        {
            _healthComponent = healthComponent;
        }

        public DeckHealthComponent GetHealthComponent()
        {
            return null;
        }
    }
}