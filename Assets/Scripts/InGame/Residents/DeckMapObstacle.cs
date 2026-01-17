using Components.Health;
using Data.Component;
using UnityEngine;
using Zenject;

namespace InGame.Residents
{
    public class DeckMapObstacle : MonoBehaviour
    {
        [SerializeField] private DeckComponentHealth componentHealth;
        [SerializeField] private DeckDataHealth dataHealth;

        [Inject]
        private void Inject(DeckComponentHealth compHealth)
        {
            componentHealth = compHealth;
        }
    }
}