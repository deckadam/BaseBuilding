using UnityEngine;

namespace Data.Component
{
    [CreateAssetMenu(fileName = "Deck Data Movement", menuName = "Deck/Data/Component/Movement")]
    public class DeckDataMovement : DeckComponentData
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float acceleration;

        public float MovementSpeed => movementSpeed;
        public float Acceleration => acceleration;
    }
}